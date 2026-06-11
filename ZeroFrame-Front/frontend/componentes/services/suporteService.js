const SuporteService = (() => {
    async function enviar(dados) {
        return ZeroFrameApi.request("/api/suporte", {
            method: "POST",
            body: dados
        });
    }

    return { enviar };
})();
