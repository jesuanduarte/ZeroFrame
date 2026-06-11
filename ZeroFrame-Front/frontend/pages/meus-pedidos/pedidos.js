document.addEventListener("DOMContentLoaded", async () => {
    if (!ZeroFrameApi.protegerPagina()) return;

    const lista = document.querySelector(".orders-list");
    if (!lista) return;

    ZeroFrameApi.mostrarMensagem(lista, "Carregando pedidos...");

    try {
        const pedidos = await PedidoService.listar();
        const pedidoConfirmado = sessionStorage.getItem("zf_pedido_confirmado");
        sessionStorage.removeItem("zf_pedido_confirmado");

        if (!pedidos.length) {
            ZeroFrameApi.mostrarMensagem(lista, "Você ainda não possui pedidos.");
            return;
        }

        lista.textContent = "";

        if (pedidoConfirmado) {
            const mensagem = document.createElement("p");
            mensagem.className = "api-feedback";
            mensagem.textContent = `Pedido #${pedidoConfirmado} criado com sucesso.`;
            lista.appendChild(mensagem);
        }

        pedidos
            .sort((a, b) => new Date(b.dataPedido || b.DataPedido || 0) - new Date(a.dataPedido || a.DataPedido || 0))
            .forEach((pedido) => lista.appendChild(criarCardPedido(pedido)));
    } catch (error) {
        ZeroFrameApi.mostrarMensagem(lista, ZeroFrameApi.tratarErro(error, "Erro ao carregar pedidos."));
    }
});

function criarCardPedido(pedido) {
    const id = pedido.id || pedido.Id || pedido.pedidoId || pedido.PedidoId;
    const statusEntrega = pedido.statusEntrega || pedido.StatusEntrega || pedido.status || pedido.Status || "Pendente";
    const statusPagamento = pedido.statusPagamento || pedido.StatusPagamento || "Pendente";
    const itens = pedido.itens || pedido.Itens || pedido.pedidoItens || pedido.PedidoItens || [];
    const total = Number(pedido.valorTotalComFrete ?? pedido.ValorTotalComFrete ?? pedido.valorTotal ?? pedido.ValorTotal ?? pedido.total ?? pedido.Total ?? 0);

    const card = document.createElement("article");
    card.className = "order-card";

    const header = document.createElement("div");
    header.className = "order-card-header";

    const titleWrapper = document.createElement("div");
    const small = document.createElement("p");
    small.className = "small-label";
    small.textContent = `Pedido #${id}`;

    const title = document.createElement("h3");
    title.textContent = getResumoPedido(itens);

    const statusEl = document.createElement("span");
    statusEl.className = `order-status ${getStatusClass(statusEntrega, statusPagamento)}`;
    statusEl.textContent = statusEntrega;

    const body = document.createElement("div");
    body.className = "order-card-body";

    const data = document.createElement("p");
    data.textContent = `Data: ${formatarData(pedido.dataPedido || pedido.DataPedido || pedido.data || pedido.Data)}`;

    const totalItens = document.createElement("p");
    totalItens.textContent = `Itens: ${Number(pedido.totalItens ?? pedido.TotalItens ?? getTotalItens(itens))}`;

    const valor = document.createElement("p");
    valor.textContent = `Valor total: ${ProdutoService.formatarPreco(total)}`;

    const entrega = document.createElement("p");
    entrega.textContent = `Entrega: ${statusEntrega}`;

    const pagamento = document.createElement("p");
    pagamento.textContent = `Pagamento: ${statusPagamento}`;

    const itensWrapper = document.createElement("div");
    itensWrapper.className = "order-items";
    preencherItensPedido(itensWrapper, itens);

    titleWrapper.append(small, title);
    header.append(titleWrapper, statusEl);
    body.append(data, totalItens, valor, entrega, pagamento, itensWrapper);
    card.append(header, body);

    return card;
}

function preencherItensPedido(container, itens) {
    if (!itens.length) {
        const vazio = document.createElement("p");
        vazio.textContent = "Itens não informados.";
        container.appendChild(vazio);
        return;
    }

    itens.forEach((item) => {
        const nome = item.nomeProduto || item.NomeProduto || item.produto?.nome || item.Produto?.Nome || "Produto Zero Frame";
        const quantidade = Number(item.quantidade ?? item.Quantidade ?? 1);
        const subtotal = Number(item.subtotal ?? item.Subtotal ?? 0);
        const tamanho = item.tamanho || item.Tamanho || "";
        const cor = item.cor || item.Cor || "";
        const variacao = [tamanho && `Tamanho: ${tamanho}`, cor && `Cor: ${cor}`].filter(Boolean).join(" | ");

        const linha = document.createElement("p");
        linha.textContent = `${nome} - ${quantidade} un.${variacao ? ` (${variacao})` : ""}${subtotal ? ` - ${ProdutoService.formatarPreco(subtotal)}` : ""}`;
        container.appendChild(linha);
    });
}

function getResumoPedido(itens) {
    const primeiro = itens[0];
    const nome = primeiro?.nomeProduto || primeiro?.NomeProduto || primeiro?.produto?.nome || primeiro?.Produto?.Nome;
    const quantidadeExtra = Math.max(0, itens.length - 1);

    if (!nome) return "Pedido Zero Frame";
    return quantidadeExtra ? `${nome} + ${quantidadeExtra} item(ns)` : nome;
}

function getTotalItens(itens) {
    return itens.reduce((total, item) => total + Number(item.quantidade ?? item.Quantidade ?? 1), 0);
}

function getStatusClass(statusEntrega, statusPagamento) {
    const entrega = String(statusEntrega || "").toLowerCase();
    const pagamento = String(statusPagamento || "").toLowerCase();
    if (entrega.includes("cancel") || pagamento.includes("cancel")) return "order-status-canceled";
    if (entrega.includes("entregue") || pagamento.includes("pago") || pagamento.includes("aprovado")) return "order-status-success";
    return "order-status-processing";
}

function formatarData(data) {
    if (!data) return "Não informada";
    return new Date(data).toLocaleDateString("pt-BR");
}

