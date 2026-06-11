const AuthService = (() => {
    async function login(email, senha) {
        const data = await ZeroFrameApi.request("/api/usuarios/login", {
            method: "POST",
            body: { email, senha }
        });

        return ZeroFrameApi.salvarSessao(data);
    }

    async function cadastrar(usuario) {
        return ZeroFrameApi.request("/api/usuarios", {
            method: "POST",
            body: usuario
        });
    }

    return {
        login,
        cadastrar,
        logout: ZeroFrameApi.limparSessao
    };
})();
