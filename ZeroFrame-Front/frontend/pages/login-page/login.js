document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector(".login-form");
    const inputs = document.querySelectorAll(".login-input");
    const mensagem = sessionStorage.getItem("zf_login_message");

    if (mensagem) {
        ZeroFrameApi.notificar(mensagem);
        sessionStorage.removeItem("zf_login_message");
    }

    if (!form) return;

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        const email = inputs[0]?.value.trim();
        const senha = inputs[1]?.value.trim();

        if (!email || !senha) {
            ZeroFrameApi.notificar("Preencha email e senha para entrar.");
            return;
        }

        try {
            const { usuario } = await AuthService.login(email, senha);
            const perfil = usuario?.perfil || localStorage.getItem("perfil");
            window.location.href = perfil === "Administrador"
                ? "../admin/admin.html"
                : "../../index.html";
        } catch (error) {
            ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Login inválido. Confira seus dados."));
        }
    });
});

document.addEventListener("DOMContentLoaded", () => {
    setupPasswordToggles();
});

function setupPasswordToggles() {
    document.querySelectorAll(".password-toggle-btn").forEach((button) => {
        const container = button.closest(".login-input-container");
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
