const CarrinhoService = (() => {
    function getUsuarioId() {
        const usuarioId = ZeroFrameApi.getUsuarioId();
        if (!usuarioId) throw new Error("Você precisa estar logado.");
        return usuarioId;
    }

    async function obter() {
        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/carrinho`);
    }

    async function listarItens() {
        const data = await ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/carrinho/itens`);
        return ZeroFrameApi.normalizarLista(data);
    }

    async function adicionar(item) {
        const variacaoProdutoId = Number(item?.variacaoProdutoId || item?.VariacaoProdutoId);
        const quantidade = Number(item?.quantidade || item?.Quantidade || 1);

        if (!variacaoProdutoId) throw new Error("Selecione uma variação válida do produto.");
        if (quantidade <= 0) throw new Error("A quantidade deve ser maior que zero.");

        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/carrinho/itens`, {
            method: "POST",
            body: { variacaoProdutoId, quantidade }
        });
    }

    async function atualizar(itemId, item) {
        const variacaoProdutoId = Number(item?.variacaoProdutoId || item?.VariacaoProdutoId);
        const quantidade = Number(item?.quantidade || item?.Quantidade);

        if (!itemId) throw new Error("Item do carrinho inválido.");
        if (!variacaoProdutoId) throw new Error("Variação do produto inválida.");
        if (quantidade <= 0) throw new Error("A quantidade deve ser maior que zero.");

        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/carrinho/itens/${itemId}`, {
            method: "PUT",
            body: {
                id: Number(itemId),
                variacaoProdutoId,
                quantidade
            }
        });
    }

    async function remover(itemId) {
        if (!itemId) throw new Error("Item do carrinho inválido.");

        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/carrinho/itens/${itemId}`, {
            method: "DELETE"
        });
    }

    function getItens(carrinho) {
        return ZeroFrameApi.normalizarLista(carrinho?.itens || carrinho?.Itens || carrinho);
    }

    function calcularTotais(itens) {
        const subtotal = itens.reduce((total, item) => {
            const precoOriginal = Number(item.precoOriginalUnitario ?? item.PrecoOriginalUnitario ?? item.precoUnitario ?? item.PrecoUnitario ?? 0);
            const quantidade = Number(item.quantidade ?? item.Quantidade ?? 1);
            const subtotalOriginal = Number(item.subtotalOriginal ?? item.SubtotalOriginal ?? precoOriginal * quantidade);
            return total + (Number.isFinite(subtotalOriginal) ? subtotalOriginal : 0);
        }, 0);
        const desconto = itens.reduce((total, item) => {
            const descontoItem = Number(item.descontoTotal ?? item.DescontoTotal ?? 0);
            return total + (Number.isFinite(descontoItem) ? descontoItem : 0);
        }, 0);
        const frete = 0;

        return {
            subtotal,
            desconto,
            frete,
            totalGeral: subtotal - desconto + frete
        };
    }

    return { obter, listarItens, adicionar, atualizar, remover, getItens, calcularTotais };
})();

