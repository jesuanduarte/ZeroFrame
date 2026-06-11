document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector(".register-form");
    const inputNome = document.querySelector("#nomeCompleto");
    const inputTelefone = document.querySelector("#telefone");
    const inputEmail = document.querySelector("#email");
    const inputSenha = document.querySelector("#senha");
    const inputConfirmarSenha = document.querySelector("#confirmarSenha");

    setupPasswordToggles();

    if (!form) return;

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const nome = inputNome?.value.trim();
        const telefone = inputTelefone?.value.trim();
        const email = inputEmail?.value.trim();
        const senha = inputSenha?.value.trim();
        const confirmarSenha = inputConfirmarSenha?.value.trim();

        if (!nome || !telefone || !email || !senha || !confirmarSenha) {
            ZeroFrameApi.notificar("Preencha todos os campos do cadastro.");
            return;
        }

        if (senha !== confirmarSenha) {
            ZeroFrameApi.notificar("As senhas precisam ser iguais.");
            return;
        }

        try {
            await AuthService.cadastrar({
                nome,
                email,
                senha,
                telefone
            });
            ZeroFrameApi.notificar("Conta criada com sucesso. Faça login para continuar.", "sucesso");
            window.location.href = "../login-page/login.html";
        } catch (error) {
            ZeroFrameApi.notificar(error.message || "Não foi possível criar a conta.");
        }
    });
});

function setupPasswordToggles() {
    document.querySelectorAll(".password-toggle-btn").forEach((button) => {
        const container = button.closest(".register-input-container");
        const input = container?.querySelector("input[type='password']");
        if (!input) return;

        button.addEventListener("click", () => {
            const showing = input.type === "text";
            input.type = showing ? "password" : "text";
            const icon = button.querySelector("i");
            if (icon) {
                icon.className = showing ? "fa-solid fa-eye-slash" : "fa-solid fa-eye";
            }
            button.setAttribute("aria-label", showing ? "Mostrar senha" : "Ocultar senha");
        });
    });
}
