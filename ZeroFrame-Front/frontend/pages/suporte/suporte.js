document.addEventListener("DOMContentLoaded", () => {
    const suporteForm = document.getElementById("suporteForm");

    if (suporteForm) {
        suporteForm.addEventListener("submit", handleFormSubmit);
    }

    applyTheme();
});

async function handleFormSubmit(event) {
    event.preventDefault();

    const formData = {
        nome: document.getElementById("nome")?.value.trim(),
        email: document.getElementById("email")?.value.trim(),
        telefone: document.getElementById("telefone")?.value.trim(),
        categoria: document.getElementById("categoria")?.value,
        assunto: document.getElementById("assunto")?.value.trim(),
        mensagem: document.getElementById("mensagem")?.value.trim(),
        data: new Date().toLocaleString("pt-BR")
    };
    const termos = document.getElementById("termos")?.checked;

    if (!formData.nome || !formData.email || !formData.assunto || !formData.mensagem || !formData.categoria) {
        mostrarErro("Preencha todos os campos obrigatorios.");
        return;
    }

    if (!termos) {
        mostrarErro("Por favor, concorde em receber respostas por email.");
        return;
    }

    if (!isValidEmail(formData.email)) {
        mostrarErro("Por favor, insira um email valido.");
        return;
    }

    await enviarFormulario(formData);
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

async function enviarFormulario(formData) {
    const btn = document.querySelector(".suporte-submit-btn");
    const btnText = btn?.textContent || "Enviar mensagem";

    if (btn) {
        btn.textContent = "Enviando...";
        btn.disabled = true;
    }

    try {
        await SuporteService.enviar(formData);
        mostrarSucesso("Mensagem enviada com sucesso! Obrigado por entrar em contato.");
        document.getElementById("suporteForm")?.reset();
    } catch (error) {
        mostrarErro(ZeroFrameApi.tratarErro(error, "Erro ao enviar. Tente novamente."));
    } finally {
        if (btn) {
            btn.textContent = btnText;
            btn.disabled = false;
        }
    }
}

function mostrarSucesso(texto) {
    mostrarFeedback(texto, "sucesso-mensagem");
}

function mostrarErro(texto) {
    mostrarFeedback(texto, "erro-mensagem");
}

function mostrarFeedback(texto, classe) {
    const container = document.querySelector(".suporte-container");
    if (!container) return;

    container.querySelectorAll(".sucesso-mensagem, .erro-mensagem").forEach((item) => item.remove());

    const mensagem = document.createElement("div");
    mensagem.className = classe;
    mensagem.textContent = texto;
    container.prepend(mensagem);

    setTimeout(() => mensagem.remove(), 5000);
}

function applyTheme() {
    if (document.body.classList.contains("modoescuro")) {
        document.body.classList.add("modoescuro");
    }
}

document.addEventListener("modoescuro-toggle", (event) => {
    document.body.classList.toggle("modoescuro", Boolean(event.detail.isDarkMode));
});

const estiloSucesso = document.createElement("style");
estiloSucesso.textContent = `
    .sucesso-mensagem {
        background: linear-gradient(135deg, #4caf50 0%, #45a049 100%);
        color: white;
        padding: 1.5rem;
        border-radius: 0.5rem;
        margin-bottom: 2rem;
        box-shadow: 0 4px 15px rgba(76, 175, 80, 0.3);
        animation: slideInDown 0.5s ease-out;
    }

    .erro-mensagem {
        background: #b3261e;
        color: white;
        padding: 1.5rem;
        border-radius: 0.5rem;
        margin-bottom: 2rem;
        box-shadow: 0 4px 15px rgba(179, 38, 30, 0.3);
        animation: slideInDown 0.5s ease-out;
    }

    @keyframes slideInDown {
        from {
            opacity: 0;
            transform: translateY(-20px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }
`;

document.head.appendChild(estiloSucesso);
