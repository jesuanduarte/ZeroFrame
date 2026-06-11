let imagensProdutoAtual = [];
let imagemAtualIndex = 0;

document.addEventListener("DOMContentLoaded", async () => {
    const params = new URLSearchParams(window.location.search);
    const produtoId = params.get("id");

    if (!produtoId) return;

    try {
        const produto = await ProdutoService.buscarPorId(produtoId);
        const variacoes = await ProdutoService.listarVariacoes(produtoId).catch(() => []);

        preencherProduto(produto, variacoes);
        configurarCarrinho(produto, variacoes);
        configurarAvaliacoes(produtoId);
        carregarRelacionados(produto);
    } catch (error) {
        const infoContainer = document.querySelector(".main-product-info-container");
        if (infoContainer) {
            infoContainer.textContent = "";
            const titulo = document.createElement("h1");
            titulo.className = "main-product-title";
            titulo.textContent = "Não encontramos este produto.";
            const mensagem = document.createElement("p");
            mensagem.textContent = ZeroFrameApi.tratarErro(error, "Produto indisponível.");
            infoContainer.append(titulo, mensagem);
        }
    }
});

function preencherProduto(produto, variacoes) {
    const categoria = ProdutoService.getCategoria(produto);
    const marca = ProdutoService.getMarca(produto);
    const imagensProduto = ProdutoService.getImagens(produto, "../../assets/products/camisa-over-black.png");
    const preco = ProdutoService.getPreco(produto);

    const categoriaEl = document.querySelector(".main-product-category");
    const tituloEl = document.querySelector(".main-product-title");
    const precoEl = document.querySelector(".main-product-price");
    const metodoEl = document.querySelector(".main-product-price-method");
    const sizeOptions = document.querySelector(".size-options");

    if (categoriaEl) categoriaEl.textContent = `${categoria} - ${marca}`;
    if (tituloEl) tituloEl.textContent = produto.nome || produto.Nome || "Produto Zero Frame";
    if (precoEl) precoEl.textContent = ProdutoService.formatarPreco(preco);
    if (metodoEl) {
        const estoque = variacoes.reduce((total, variacao) => total + Number(variacao.estoque ?? variacao.Estoque ?? 0), 0);
        metodoEl.textContent = `${produto.descricao || produto.Descricao || "no pix ou 10x sem juros"}${estoque ? ` | Estoque: ${estoque}` : " | Sem estoque"}`;
    }

    configurarCarrosselProduto(imagensProduto, produto.nome || produto.Nome || "Produto Zero Frame");

    if (sizeOptions && variacoes.length) {
        sizeOptions.textContent = "";

        variacoes.forEach((variacao, index) => {
            const estoque = Number(variacao.estoque ?? variacao.Estoque ?? 0);
            const button = document.createElement("button");
            button.className = "size-option";
            button.dataset.variacaoId = variacao.id || variacao.Id || variacao.variacaoProdutoId || variacao.VariacaoProdutoId;
            button.dataset.estoque = String(estoque);
            button.dataset.cor = variacao.cor || variacao.Cor || "";
            button.textContent = variacao.tamanho || variacao.Tamanho || `Opção ${index + 1}`;
            button.disabled = estoque <= 0;

            if (!sizeOptions.querySelector(".size-option:not(:disabled)") && estoque > 0) {
                button.classList.add("selected");
            }

            sizeOptions.appendChild(button);
        });
    }
}

function configurarCarrosselProduto(imagens, nomeProduto) {
    imagensProdutoAtual = [...new Set(imagens.filter(Boolean))];
    imagemAtualIndex = 0;

    renderizarImagemPrincipal(nomeProduto);
    renderizarMiniaturasProduto(nomeProduto);
    configurarBotoesCarrossel(nomeProduto);
}

function renderizarImagemPrincipal(nomeProduto) {
    const imagemPrincipal = document.getElementById("product-main-image");
    const imagem = imagensProdutoAtual[imagemAtualIndex];
    if (!imagemPrincipal || !imagem) return;

    // A imagem principal sempre usa o array real do produto aberto, sem buscar imagens de outros cards.
    imagemPrincipal.src = imagem;
    imagemPrincipal.alt = imagensProdutoAtual.length > 1
        ? `${nomeProduto} - imagem ${imagemAtualIndex + 1}`
        : nomeProduto;
    imagemPrincipal.onerror = () => {
        imagemPrincipal.src = "../../assets/products/camisa-over-black.png";
    };

    document.querySelectorAll(".product-thumbnail").forEach((thumb, index) => {
        thumb.classList.toggle("selected", index === imagemAtualIndex);
    });
}

function configurarBotoesCarrossel(nomeProduto) {
    const prev = document.getElementById("prev-button");
    const next = document.getElementById("next-button");
    const mostrarControles = imagensProdutoAtual.length > 1;

    if (prev) {
        prev.style.display = mostrarControles ? "" : "none";
        prev.onclick = () => trocarImagemProduto(-1, nomeProduto);
    }

    if (next) {
        next.style.display = mostrarControles ? "" : "none";
        next.onclick = () => trocarImagemProduto(1, nomeProduto);
    }
}

function trocarImagemProduto(direcao, nomeProduto) {
    if (imagensProdutoAtual.length <= 1) return;

    imagemAtualIndex = (imagemAtualIndex + direcao + imagensProdutoAtual.length) % imagensProdutoAtual.length;
    renderizarImagemPrincipal(nomeProduto);
}

function renderizarMiniaturasProduto(nomeProduto) {
    const thumbnails = document.querySelector(".product-thumbnails");
    if (!thumbnails) return;

    thumbnails.textContent = "";
    thumbnails.style.display = imagensProdutoAtual.length > 1 ? "" : "none";
    if (imagensProdutoAtual.length <= 1) return;

    // Miniaturas geradas exclusivamente das imagens cadastradas no produto atual.
    imagensProdutoAtual.forEach((imagem, index) => {
        const button = document.createElement("button");
        button.type = "button";
        button.className = `product-thumbnail${index === imagemAtualIndex ? " selected" : ""}`;
        button.setAttribute("aria-label", `Ver imagem ${index + 1}`);

        const img = document.createElement("img");
        img.src = imagem;
        img.alt = `${nomeProduto} - miniatura ${index + 1}`;
        img.onerror = () => {
            img.src = "../../assets/products/camisa-over-black.png";
        };

        button.appendChild(img);
        button.addEventListener("click", () => {
            imagemAtualIndex = index;
            renderizarImagemPrincipal(nomeProduto);
        });

        thumbnails.appendChild(button);
    });
}

async function configurarAvaliacoes(produtoId) {
    const summary = document.querySelector(".reviews-summary");
    const list = document.querySelector(".reviews-list");
    const form = document.querySelector(".review-form");

    if (!summary || !list || !form || typeof AvaliacaoService === "undefined") return;

    if (!ZeroFrameApi.estaLogado()) {
        form.style.display = "none";
    }

    form.addEventListener("submit", async (event) => {
        event.preventDefault();

        if (!ZeroFrameApi.estaLogado()) {
            ZeroFrameApi.redirecionarParaLogin("Entre na sua conta para avaliar produtos.");
            return;
        }

        const formData = new FormData(form);
        const nota = formData.get("nota");
        const comentario = formData.get("comentario")?.toString().trim() || "";

        try {
            await AvaliacaoService.criar(produtoId, nota, comentario);
            ZeroFrameApi.notificar("Avaliação enviada com sucesso.", "sucesso");
            form.reset();
            await carregarAvaliacoes(produtoId);
        } catch (error) {
            const mensagem = ZeroFrameApi.tratarErro(error, "Não foi possível enviar a avaliação.");
            if (String(mensagem).toLowerCase().includes("ja avaliou") || String(mensagem).toLowerCase().includes("já avaliou")) {
                try {
                    await AvaliacaoService.atualizarMinha(produtoId, nota, comentario);
                    ZeroFrameApi.notificar("Avaliação atualizada com sucesso.", "sucesso");
                    form.reset();
                    await carregarAvaliacoes(produtoId);
                    return;
                } catch (updateError) {
                    ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(updateError, "Não foi possível atualizar a avaliação."));
                    return;
                }
            }
            ZeroFrameApi.notificar(mensagem);
        }
    });

    await carregarAvaliacoes(produtoId);
}

async function carregarAvaliacoes(produtoId) {
    const summary = document.querySelector(".reviews-summary");
    const list = document.querySelector(".reviews-list");
    const rating = document.querySelector(".main-product-rating");
    if (!summary || !list) return;

    summary.textContent = "Carregando avaliações...";
    list.textContent = "";

    try {
        const [avaliacoes, resumo] = await Promise.all([
            AvaliacaoService.listar(produtoId),
            AvaliacaoService.resumo(produtoId).catch(() => null)
        ]);

        const media = resumo?.mediaAvaliacoes ?? resumo?.MediaAvaliacoes ?? 0;
        const total = resumo?.totalAvaliacoes ?? resumo?.TotalAvaliacoes ?? avaliacoes.length;
        summary.textContent = total
            ? `${Number(media).toFixed(1)} de 5 estrelas (${total} avaliação${total === 1 ? "" : "s"})`
            : "Este produto ainda não possui avaliações.";

        if (rating) {
            rating.textContent = total ? `★ ${Number(media).toFixed(1)} (${total})` : "★ Sem avaliações";
        }

        if (!avaliacoes.length) {
            const empty = document.createElement("p");
            empty.textContent = "Nenhum comentário publicado ainda.";
            list.appendChild(empty);
            return;
        }

        avaliacoes.forEach((avaliacao) => {
            const card = document.createElement("article");
            card.className = "review-card";

            const stars = document.createElement("p");
            stars.className = "review-stars";
            const nota = Number(avaliacao.nota ?? avaliacao.Nota ?? 0);
            stars.textContent = "★".repeat(Math.max(1, Math.round(nota)));

            const author = document.createElement("strong");
            author.textContent = avaliacao.nomeUsuario || avaliacao.NomeUsuario || "Cliente";

            const comment = document.createElement("p");
            comment.textContent = avaliacao.comentario || avaliacao.Comentario || "Sem comentário.";

            card.append(stars, author, comment);
            list.appendChild(card);
        });
    } catch (error) {
        summary.textContent = ZeroFrameApi.tratarErro(error, "Erro ao carregar avaliações.");
    }
}

function configurarCarrinho(produto, variacoes) {
    const btn = document.querySelector(".main-add-to-cart-btn");
    const comprarBtn = document.querySelector(".main-buy-btn");

    async function adicionarProdutoAoCarrinho() {
        if (!ZeroFrameApi.estaLogado()) {
            ZeroFrameApi.redirecionarParaLogin("Entre na sua conta para adicionar produtos ao carrinho.");
            return false;
        }

        const selecionado = document.querySelector(".size-option.selected") || document.querySelector(".size-option:not(:disabled)");
        const variacaoId = selecionado?.dataset?.variacaoId || getPrimeiraVariacaoDisponivel(variacoes)?.id || getPrimeiraVariacaoDisponivel(variacoes)?.Id;

        if (!variacaoId) {
            ZeroFrameApi.notificar("Produto sem variação disponível para compra.");
            return false;
        }

        if (Number(selecionado?.dataset?.estoque ?? 1) <= 0) {
            ZeroFrameApi.notificar("Produto sem estoque suficiente.");
            return false;
        }

        try {
            await CarrinhoService.adicionar({
                variacaoProdutoId: Number(variacaoId),
                quantidade: 1
            });
            return true;
        } catch (error) {
            ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Não foi possível adicionar o produto ao carrinho."));
            return false;
        }
    }

    btn?.addEventListener("click", async () => {
        const adicionado = await adicionarProdutoAoCarrinho();
        if (adicionado) {
            ZeroFrameApi.notificar("Produto adicionado ao carrinho.", "sucesso");
            window.atualizarCarrinhoBadge?.();
        }
    });

    comprarBtn?.addEventListener("click", async () => {
        const adicionado = await adicionarProdutoAoCarrinho();
        if (adicionado) {
            window.atualizarCarrinhoBadge?.();
            window.location.href = "../carrinho/carrinho.html";
        }
    });

    document.addEventListener("click", (event) => {
        if (!event.target.classList.contains("size-option") || event.target.disabled) return;
        document.querySelectorAll(".size-option").forEach((button) => button.classList.remove("selected"));
        event.target.classList.add("selected");
    });
}

async function carregarRelacionados(produto) {
    const containers = document.querySelectorAll(".products-container");
    if (!containers.length) return;

    try {
        const relacionados = await ProdutoService.listar({
            Categoria: ProdutoService.getCategoria(produto)
        });

        if (!relacionados.length) return;

        containers.forEach((container) => {
            container.textContent = "";
            relacionados.slice(0, 7).forEach((item) => {
                container.appendChild(ProdutoService.criarCard(item, {
                    productPath: "../../pages/produtos/product.html",
                    fallbackImage: "../../assets/products/camisa-over-black.png"
                }));
            });
        });
    } catch {
        // Produtos relacionados são opcionais; a compra do produto atual continua funcionando.
    }
}

function getPrimeiraVariacaoDisponivel(variacoes) {
    return variacoes.find((variacao) => Number(variacao.estoque ?? variacao.Estoque ?? 0) > 0);
}
