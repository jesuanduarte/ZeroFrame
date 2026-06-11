document.addEventListener("DOMContentLoaded", () => {
    if (!ZeroFrameApi.protegerPagina()) return;

    const personalInfoForm = document.getElementById("personalInfoForm");
    const securityForm = document.getElementById("securityForm");
    const themeToggle = document.getElementById("themeToggle");
    const pageNotice = document.getElementById("pageNotice");
    const logout = document.getElementById("logout");

    preencherDadosUsuario();

    function showNotice(message, type = "success") {
        if (!pageNotice) return;

        const notice = document.createElement("div");
        notice.className = `notice ${type}`;
        notice.textContent = message;

        pageNotice.appendChild(notice);
        setTimeout(() => {
            notice.remove();
        }, 3800);
    }

    function getUsuarioAtual() {
        return ZeroFrameApi.getUsuario() || {};
    }

    function preencherDadosUsuario(usuario = getUsuarioAtual()) {
        if (!usuario || !personalInfoForm) return;

        const nome = personalInfoForm.querySelector("input[name='fullName']");
        const email = personalInfoForm.querySelector("input[name='email']");
        const telefone = personalInfoForm.querySelector("input[name='phone']");

        if (nome) nome.value = usuario.nome || usuario.Nome || "";
        if (email) email.value = usuario.email || usuario.Email || "";
        if (telefone) telefone.value = usuario.telefone || usuario.Telefone || "";
    }

    function montarDadosBase() {
        const usuario = getUsuarioAtual();
        return {
            nome: personalInfoForm?.querySelector("input[name='fullName']")?.value.trim() || usuario.nome || usuario.Nome || "",
            email: personalInfoForm?.querySelector("input[name='email']")?.value.trim() || usuario.email || usuario.Email || "",
            telefone: personalInfoForm?.querySelector("input[name='phone']")?.value.trim() || usuario.telefone || usuario.Telefone || ""
        };
    }

    function atualizarSessaoUsuario(usuarioAtualizado) {
        const usuarioAtual = getUsuarioAtual();
        const token = ZeroFrameApi.getToken();
        const usuarioSeguro = {
            ...usuarioAtual,
            ...usuarioAtualizado,
            id: usuarioAtualizado.id || usuarioAtualizado.Id || usuarioAtual.id || usuarioAtual.Id || usuarioAtual.usuarioId || usuarioAtual.UsuarioId,
            usuarioId: usuarioAtualizado.id || usuarioAtualizado.Id || usuarioAtual.usuarioId || usuarioAtual.UsuarioId || usuarioAtual.id || usuarioAtual.Id,
            nome: usuarioAtualizado.nome || usuarioAtualizado.Nome || usuarioAtual.nome || usuarioAtual.Nome,
            email: usuarioAtualizado.email || usuarioAtualizado.Email || usuarioAtual.email || usuarioAtual.Email,
            telefone: usuarioAtualizado.telefone || usuarioAtualizado.Telefone || usuarioAtual.telefone || usuarioAtual.Telefone,
            perfil: usuarioAtual.perfil || usuarioAtual.Perfil || usuarioAtualizado.perfil || usuarioAtualizado.Perfil,
            ativo: usuarioAtualizado.ativo ?? usuarioAtualizado.Ativo ?? usuarioAtual.ativo ?? usuarioAtual.Ativo
        };

        ZeroFrameApi.salvarSessao({ token, usuario: usuarioSeguro });
        preencherDadosUsuario(usuarioSeguro);
    }

    if (personalInfoForm) {
        personalInfoForm.addEventListener("submit", async (event) => {
            event.preventDefault();

            try {
                const usuarioAtualizado = await UsuarioService.atualizarMeusDados(montarDadosBase());
                atualizarSessaoUsuario(usuarioAtualizado || {});
                showNotice("Dados atualizados com sucesso.", "success");
            } catch (error) {
                showNotice(ZeroFrameApi.tratarErro(error, "Nao foi possivel atualizar seus dados."), "error");
            }
        });
    }

    if (securityForm) {
        securityForm.addEventListener("submit", async (event) => {
            event.preventDefault();

            const senhaAtual = securityForm.querySelector("input[name='currentPassword']")?.value || "";
            const novaSenha = securityForm.querySelector("input[name='newPassword']")?.value || "";
            const confirmarSenha = securityForm.querySelector("input[name='confirmPassword']")?.value || "";

            if (novaSenha !== confirmarSenha) {
                showNotice("A confirmacao da senha nao confere.", "error");
                return;
            }

            try {
                const usuarioAtualizado = await UsuarioService.atualizarMeusDados({
                    ...montarDadosBase(),
                    senhaAtual,
                    novaSenha
                });
                atualizarSessaoUsuario(usuarioAtualizado || {});
                securityForm.reset();
                showNotice("Senha atualizada com sucesso.", "success");
            } catch (error) {
                showNotice(ZeroFrameApi.tratarErro(error, "Nao foi possivel atualizar a senha."), "error");
            }
        });
    }

    logout?.addEventListener("click", () => {
        ZeroFrameApi.limparSessao();
        window.location.href = "../login-page/login.html";
    });

    setupPasswordToggles();

    if (themeToggle) {
        themeToggle.addEventListener("change", () => {
            const isDark = themeToggle.checked;
            document.body.dataset.theme = isDark ? "dark" : "light";
            showNotice(`Modo ${isDark ? "escuro" : "claro"} ativado.`, "success");
        });
    }

    function setupPasswordToggles() {
        document.querySelectorAll(".password-toggle-btn").forEach((button) => {
            const container = button.closest(".password-input-container");
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
});
