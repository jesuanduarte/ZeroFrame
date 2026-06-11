const PedidoService = (() => {
    function getUsuarioId() {
        const usuarioId = ZeroFrameApi.getUsuarioId();
        if (!usuarioId) throw new Error("Você precisa estar logado.");
        return usuarioId;
    }

    async function listar() {
        const data = await ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/pedidos`);
        return ZeroFrameApi.normalizarLista(data);
    }

    async function criar(enderecoId) {
        const id = Number(enderecoId);
        if (!id) throw new Error("Cadastre ou selecione um endereco antes de finalizar a compra.");

        return ZeroFrameApi.request(`/api/usuarios/${getUsuarioId()}/pedidos`, {
            method: "POST",
            body: { enderecoId: id }
        });
    }

    async function criarAPartirDoCarrinho(enderecoId) {
        return criar(enderecoId);
    }

    async function adicionarItem(pedidoId, item) {
        const variacaoProdutoId = item?.variacaoProdutoId || item?.variacaoProduto?.id;
        const quantidade = Number(item?.quantidade || 1);

        return ZeroFrameApi.request(`/api/pedidos/${pedidoId}/itens`, {
            method: "POST",
            body: {
                variacaoProdutoId: Number(variacaoProdutoId),
                quantidade
            }
        });
    }

    async function pagar(pedidoId, metodo) {
        return ZeroFrameApi.request(`/api/pedidos/${pedidoId}/pagamento`, {
            method: "POST",
            body: { metodo }
        });
    }

    return { listar, criar, criarAPartirDoCarrinho, adicionarItem, pagar };
})();
