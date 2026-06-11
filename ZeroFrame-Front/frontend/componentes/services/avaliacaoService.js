const AvaliacaoService = (() => {
    async function listar(produtoId) {
        const data = await ZeroFrameApi.request(`/api/produtos/${produtoId}/avaliacoes`);
        return ZeroFrameApi.normalizarLista(data);
    }

    async function resumo(produtoId) {
        return ZeroFrameApi.request(`/api/produtos/${produtoId}/avaliacoes/resumo`);
    }

    async function criar(produtoId, nota, comentario) {
        return ZeroFrameApi.request(`/api/produtos/${produtoId}/avaliacoes`, {
            method: "POST",
            body: {
                produtoId: Number(produtoId),
                nota: Number(nota),
                comentario
            }
        });
    }

    async function atualizarMinha(produtoId, nota, comentario) {
        return ZeroFrameApi.request(`/api/produtos/${produtoId}/avaliacoes/minha-avaliacao`, {
            method: "PUT",
            body: {
                nota: Number(nota),
                comentario
            }
        });
    }

    return { listar, resumo, criar, atualizarMinha };
})();
