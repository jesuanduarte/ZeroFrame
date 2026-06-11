const UsuarioService = (() => {
    async function atualizarMeusDados(dados) {
        return ZeroFrameApi.request("/api/usuarios/meus-dados", {
            method: "PUT",
            body: dados
        });
    }

    return { atualizarMeusDados };
})();
