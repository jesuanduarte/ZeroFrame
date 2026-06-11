document.addEventListener("DOMContentLoaded", async () => {
    if (!ZeroFrameApi.protegerPagina()) return;

    const lista = document.querySelector(".carrinho-itens-container");
    const resumo = document.querySelector(".carrinho-summary");
    const finalizarBtn = document.querySelector(".finalizar-compra-button");
    let itensAtuais = [];

    await carregarCarrinho();

    if (lista) {
        lista.addEventListener("click", async (event) => {
            const removerBtn = event.target.closest("[data-remover-item]");
            if (!removerBtn) return;

            try {
                await CarrinhoService.remover(removerBtn.dataset.removerItem);
                await carregarCarrinho();
            } catch (error) {
                mostrarErro(error, "Não foi possível remover o item.");
            }
        });

        lista.addEventListener("change", async (event) => {
            if (!event.target.matches("[data-quantidade-item]")) return;

            const quantidade = Number(event.target.value);
            if (quantidade <= 0) {
                event.target.value = 1;
                ZeroFrameApi.notificar("A quantidade deve ser maior que zero.");
                return;
            }

            try {
                await CarrinhoService.atualizar(event.target.dataset.quantidadeItem, {
                    variacaoProdutoId: Number(event.target.dataset.variacaoId),
                    quantidade
                });
                await carregarCarrinho();
            } catch (error) {
                mostrarErro(error, "Não foi possível atualizar a quantidade.");
                await carregarCarrinho();
            }
        });
    }

    finalizarBtn?.addEventListener("click", finalizarCompra);

    async function carregarCarrinho() {
        if (!lista) return;

        ZeroFrameApi.mostrarMensagem(lista, "Carregando carrinho...");

        try {
            const carrinho = await CarrinhoService.obter();
            itensAtuais = CarrinhoService.getItens(carrinho);
            renderizarItens(itensAtuais);
            renderizarResumo(carrinho, itensAtuais);
        } catch (error) {
            itensAtuais = [];
            ZeroFrameApi.mostrarMensagem(lista, ZeroFrameApi.tratarErro(error, "Erro ao carregar carrinho."));
            renderizarResumo(null, []);
        }
    }

    function renderizarItens(itens) {
        if (!lista) return;

        lista.textContent = "";

        if (!itens.length) {
            ZeroFrameApi.mostrarMensagem(lista, "Carrinho vazio.");
            return;
        }

        itens.forEach((item) => {
            const dados = normalizarItem(item);
            const card = document.createElement("li");
            card.className = "carrinho-item-card";

            const checkbox = document.createElement("input");
            checkbox.type = "checkbox";
            checkbox.className = "carrinho-item-checkbox";
            checkbox.checked = true;

            const image = document.createElement("img");
            image.src = dados.imagem;
            image.alt = dados.nome;
            image.className = "carrinho-item-image";
            image.onerror = () => {
                image.src = "../../assets/products/aj1-high-latte.png";
            };

            const details = document.createElement("div");
            details.className = "carrinho-item-details";

            const categoria = document.createElement("p");
            categoria.textContent = `${dados.categoria} - ${dados.marca}`;

            const nome = document.createElement("h2");
            nome.textContent = dados.nome;

            const variacao = document.createElement("p");
            variacao.textContent = dados.variacao;

            const preco = document.createElement("h3");
            preco.className = "carrinho-item-price";
            preco.textContent = ProdutoService.formatarPreco(dados.preco);

            const subtotal = document.createElement("p");
            subtotal.textContent = `Subtotal: ${ProdutoService.formatarPreco(dados.subtotal)}`;

            const label = document.createElement("label");
            label.textContent = "Qtd. ";

            const quantidade = document.createElement("input");
            quantidade.type = "number";
            quantidade.min = "1";
            quantidade.value = dados.quantidade;
            quantidade.dataset.quantidadeItem = dados.itemId;
            quantidade.dataset.variacaoId = dados.variacaoId;

            const remover = document.createElement("button");
            remover.type = "button";
            remover.dataset.removerItem = dados.itemId;
            remover.textContent = "Remover";

            label.appendChild(quantidade);
            details.append(categoria, nome, variacao, preco, subtotal, label, remover);
            card.append(checkbox, image, details);

            lista.appendChild(card);
        });
    }

    function renderizarResumo(carrinho, itens) {
        if (!resumo) return;

        const numeroSeguro = (valor) => {
            const numero = Number(valor);
            return Number.isFinite(numero) ? numero : 0;
        };
        const totaisApi = {
            subtotal: numeroSeguro(carrinho?.subtotal ?? carrinho?.Subtotal),
            desconto: numeroSeguro(carrinho?.desconto ?? carrinho?.Desconto),
            frete: numeroSeguro(carrinho?.frete ?? carrinho?.Frete ?? carrinho?.valorFrete ?? carrinho?.ValorFrete),
            totalGeral: numeroSeguro(carrinho?.totalGeral ?? carrinho?.TotalGeral ?? carrinho?.valorTotalComFrete ?? carrinho?.ValorTotalComFrete)
        };
        const totaisCalculados = CarrinhoService.calcularTotais(itens);
        const subtotal = totaisApi.subtotal > 0 ? totaisApi.subtotal : totaisCalculados.subtotal;
        const desconto = totaisApi.desconto > 0 ? totaisApi.desconto : totaisCalculados.desconto;
        const frete = totaisApi.frete > 0 ? totaisApi.frete : totaisCalculados.frete;
        const totalFinal = Math.max(0, subtotal - desconto + frete);

        resumo.querySelector(".total-itens p:last-child").textContent = ProdutoService.formatarPreco(subtotal);
        resumo.querySelector(".total-desconto p:last-child").textContent = ProdutoService.formatarPreco(desconto);
        resumo.querySelector(".subtotal p:last-child").textContent = ProdutoService.formatarPreco(subtotal - desconto);
        resumo.querySelector(".frete p:last-child").textContent = frete > 0 ? ProdutoService.formatarPreco(frete) : "Grátis";
        resumo.querySelector(".total-geral p:last-child").textContent = ProdutoService.formatarPreco(totalFinal);

        if (finalizarBtn) {
            finalizarBtn.disabled = !itens.length;
        }
    }

    async function finalizarCompra() {
        if (!itensAtuais.length) {
            ZeroFrameApi.notificar("Carrinho vazio.");
            return;
        }

        try {
            finalizarBtn.disabled = true;
            finalizarBtn.textContent = "Finalizando...";

            // O checkout usa o endereço cadastrado como validação mínima antes de criar o pedido.
            const endereco = await EnderecoService.obterPrincipal();
            if (!endereco) {
                ZeroFrameApi.notificar("Cadastre um endereço antes de finalizar a compra.");
                window.location.href = "../meus-enderecos/enderecos.html";
                return;
            }

            const enderecoId = endereco.id || endereco.Id || endereco.enderecoId || endereco.EnderecoId;
            const pedido = await PedidoService.criarAPartirDoCarrinho(enderecoId);
            sessionStorage.setItem("zf_pedido_confirmado", String(pedido?.id || pedido?.pedidoId || ""));
            window.location.href = "../meus-pedidos/pedidos.html";
        } catch (error) {
            mostrarErro(error, "Não foi possível finalizar o pedido.");
        } finally {
            if (finalizarBtn) {
                finalizarBtn.disabled = false;
                finalizarBtn.textContent = "Finalizar compra";
            }
        }
    }

    function normalizarItem(item) {
        const produto = item.produto || item.Produto || item.variacaoProduto?.produto || item.VariacaoProduto?.Produto || {};
        const nome = item.nomeProduto || item.NomeProduto || produto.nome || produto.Nome || "Produto Zero Frame";
        const preco = numeroSeguro(item.precoUnitario ?? item.PrecoUnitario ?? produto.precoFinal ?? produto.PrecoFinal ?? produto.preco ?? produto.Preco);
        const quantidade = numeroSeguro(item.quantidade ?? item.Quantidade ?? 1) || 1;
        const subtotal = numeroSeguro(item.subtotal ?? item.Subtotal ?? preco * quantidade);
        const imagem = item.imagemUrl || item.ImagemUrl || produto.imagemUrl || produto.ImagemUrl || produto.imagem || produto.Imagem;
        const tamanho = item.tamanho || item.Tamanho || "";
        const cor = item.cor || item.Cor || "";

        return {
            itemId: item.id || item.Id || item.itemCarrinhoId || item.ItemCarrinhoId,
            variacaoId: item.variacaoProdutoId || item.VariacaoProdutoId || item.variacaoProduto?.id || item.VariacaoProduto?.Id,
            nome,
            preco,
            quantidade,
            subtotal,
            // Usa a mesma normalizacao central dos cards para corrigir uploads da API e evitar imagem quebrada no carrinho.
            imagem: ProdutoService.getImagem({ ...produto, imagemUrl: imagem }, "../../assets/products/aj1-high-latte.png"),
            categoria: item.categoriaNome || item.CategoriaNome || produto.categoria?.nome || produto.Categoria?.Nome || "Produto",
            marca: item.marca || item.Marca || item.origem || item.Origem || produto.marca || produto.Marca || "Zero Frame",
            variacao: [tamanho && `Tamanho: ${tamanho}`, cor && `Cor: ${cor}`].filter(Boolean).join(" | ")
        };
    }

    function numeroSeguro(valor) {
        const numero = Number(valor);
        return Number.isFinite(numero) ? numero : 0;
    }

    function mostrarErro(error, fallback) {
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, fallback));
    }
});

