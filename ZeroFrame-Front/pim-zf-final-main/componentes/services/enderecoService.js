const EnderecoService = (() => {
    function getUsuarioId() {
        const usuarioId = ZeroFrameApi.getUsuarioId();
        if (!usuarioId) throw new Error("Você precisa estar logado.");
        return usuarioId;
    }

    async function listar() {
        try {
            const data = await ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/endereco`);
            return Array.isArray(data) ? data : data ? [data] : [];
        } catch (error) {
            const mensagem = error.message?.toLowerCase() || "";
            if (error.status === 404 || mensagem.includes("endereco") || mensagem.includes("endereço")) {
                return [];
            }

            throw error;
        }
    }

    async function obterPrincipal() {
        const enderecos = await listar();
        return enderecos.find((endereco) => endereco.ativo !== false && endereco.Ativo !== false) || enderecos[0] || null;
    }

    async function adicionar(endereco) {
        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/endereco`, {
            method: "POST",
            body: montarDto(endereco)
        });
    }

    async function editar(enderecoId, endereco) {
        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/endereco/${enderecoId}`, {
            method: "PUT",
            body: { ...montarDto(endereco), id: Number(enderecoId), ativo: endereco.ativo ?? endereco.Ativo ?? true }
        });
    }

    async function remover(enderecoId) {
        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/endereco/${enderecoId}`, {
            method: "DELETE"
        });
    }

    function montarDto(endereco) {
        return {
            rua: endereco.rua || "",
            numero: endereco.numero || "",
            bairro: endereco.bairro || "",
            cidade: endereco.cidade || "",
            estado: String(endereco.estado || "").toUpperCase().slice(0, 2),
            cep: endereco.cep || "",
            complemento: endereco.complemento || "",
            usuarioId: Number(getUsuarioId())
        };
    }

    return { listar, obterPrincipal, adicionar, editar, remover };
})();
