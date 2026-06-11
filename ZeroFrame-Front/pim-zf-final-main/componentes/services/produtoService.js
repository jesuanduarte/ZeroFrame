const ProdutoService = (() => {
    function montarQuery(filtros = {}) {
        const params = new URLSearchParams();

        Object.entries(filtros).forEach(([chave, valor]) => {
            if (valor !== undefined && valor !== null && valor !== "") {
                params.append(chave, valor);
            }
        });

        return params.toString();
    }

    async function listar(filtros = {}) {
        const query = montarQuery(filtros);
        const data = await ZeroFrameApi.request(`/api/produtos${query ? `?${query}` : ""}`);
        return ZeroFrameApi.normalizarLista(data);
    }

    async function buscarPorId(id) {
        return ZeroFrameApi.request(`/api/produtos/${id}`);
    }

    async function listarVariacoes(produtoId) {
        const data = await ZeroFrameApi.request(`/api/produtos/${produtoId}/variacoes`);
        return ZeroFrameApi.normalizarLista(data);
    }

    function getProdutoId(produto) {
        return produto?.id || produto?.Id || produto?.produtoId || produto?.ProdutoId;
    }

    function getCategoria(produto) {
        return produto?.categoria?.nome || produto?.Categoria?.Nome || produto?.categoriaNome || produto?.CategoriaNome || produto?.categoria || produto?.Categoria || "Produto";
    }

    function getMarca(produto) {
        return produto?.marca || produto?.Marca || produto?.origem || produto?.Origem || "Zero Frame";
    }

    function normalizarListaImagens(valor) {
        if (!valor) return [];
        if (Array.isArray(valor)) return valor;

        if (typeof valor === "string") {
            const texto = valor.trim();
            if (!texto) return [];

            try {
                const parsed = JSON.parse(texto);
                if (Array.isArray(parsed)) return parsed;
            } catch {
                // Mantem compatibilidade com APIs que retornam uma unica URL em string.
            }

            return texto.split(/[;,|]/).map((item) => item.trim()).filter(Boolean);
        }

        return [];
    }

    function getImagens(produto, caminhoPadrao) {
        const imagensCandidatas = [
            produto?.imagens,
            produto?.Imagens,
            produto?.imagensUrl,
            produto?.ImagensUrl,
            produto?.imagemUrls,
            produto?.ImagemUrls,
            produto?.galeria,
            produto?.Galeria,
            produto?.fotos,
            produto?.Fotos,
            produto?.produtoImagens,
            produto?.ProdutoImagens,
            produto?.imagensProduto,
            produto?.ImagensProduto
        ];

        const urls = imagensCandidatas
            .flatMap(normalizarListaImagens)
            .map((item) => typeof item === "string"
                ? item
                : item?.url || item?.Url || item?.imagemUrl || item?.ImagemUrl || item?.caminho || item?.Caminho || item?.path || item?.Path || item?.src || item?.Src)
            .filter(Boolean);

        const imagemPrincipal = produto?.imagemUrl || produto?.ImagemUrl || produto?.imagem || produto?.Imagem || produto?.urlImagem || produto?.UrlImagem;
        // Tambem interpreta ImagemUrl quando a API salva varias URLs em JSON; a primeira continua sendo principal.
        urls.push(...normalizarListaImagens(imagemPrincipal));

        // Remove duplicadas e usa a normalizacao central para exibir apenas imagens do produto recebido.
        const unicas = [...new Set(urls.map((url) => String(url).trim()).filter(Boolean))];
        return unicas.length
            ? unicas.map((url) => ZeroFrameApi.getProductImageUrl(url, caminhoPadrao))
            : [ZeroFrameApi.getProductImageUrl("", caminhoPadrao)];
    }

    function getImagem(produto, caminhoPadrao) {
        return getImagens(produto, caminhoPadrao)[0];
    }

    function getPreco(produto) {
        return Number(produto?.precoFinal || produto?.PrecoFinal || produto?.preco || produto?.Preco || produto?.valor || produto?.Valor || produto?.precoAtual || produto?.PrecoAtual || 0);
    }

    function getEstoque(produto) {
        const variacoes = produto?.variacoes || produto?.Variacoes || [];
        if (Array.isArray(variacoes) && variacoes.length) {
            return variacoes.reduce((total, variacao) => total + Number(variacao?.estoque ?? variacao?.Estoque ?? 0), 0);
        }

        return Number(produto?.estoque ?? produto?.Estoque ?? 0);
    }

    function formatarPreco(valor) {
        return Number(valor || 0).toLocaleString("pt-BR", {
            style: "currency",
            currency: "BRL"
        });
    }

    function renderCard(produto, opcoes = {}) {
        return criarCard(produto, opcoes).outerHTML;
    }

    function abrirGaleriaProduto(imagens, nomeProduto) {
        const imagensGaleria = imagens.slice(1);
        if (!imagensGaleria.length) return;

        let indiceAtual = 0;
        let modal = document.querySelector(".product-gallery-modal");

        if (!modal) {
            modal = document.createElement("div");
            modal.className = "product-gallery-modal";
            modal.innerHTML = `
                <div class="product-gallery-dialog" role="dialog" aria-modal="true">
                    <button type="button" class="product-gallery-close" aria-label="Fechar galeria">&times;</button>
                    <button type="button" class="product-gallery-nav product-gallery-prev" aria-label="Imagem anterior">&#8249;</button>
                    <img class="product-gallery-image" alt="">
                    <button type="button" class="product-gallery-nav product-gallery-next" aria-label="Proxima imagem">&#8250;</button>
                    <p class="product-gallery-count"></p>
                </div>
            `;
            document.body.appendChild(modal);

            modal.addEventListener("click", (event) => {
                if (event.target === modal || event.target.classList.contains("product-gallery-close")) {
                    modal.classList.remove("is-open");
                }
            });
        }

        const img = modal.querySelector(".product-gallery-image");
        const count = modal.querySelector(".product-gallery-count");
        const prev = modal.querySelector(".product-gallery-prev");
        const next = modal.querySelector(".product-gallery-next");

        function atualizarGaleria() {
            img.src = imagensGaleria[indiceAtual];
            img.alt = `${nomeProduto} - imagem ${indiceAtual + 2}`;
            count.textContent = `${indiceAtual + 1} de ${imagensGaleria.length}`;
            const mostrarSetas = imagensGaleria.length > 1;
            prev.style.display = mostrarSetas ? "" : "none";
            next.style.display = mostrarSetas ? "" : "none";
        }

        prev.onclick = () => {
            indiceAtual = (indiceAtual - 1 + imagensGaleria.length) % imagensGaleria.length;
            atualizarGaleria();
        };

        next.onclick = () => {
            indiceAtual = (indiceAtual + 1) % imagensGaleria.length;
            atualizarGaleria();
        };

        // Mostra somente imagens extras do produto clicado, sem reaproveitar imagens de outros cards.
        atualizarGaleria();
        modal.classList.add("is-open");
    }

    function criarCard(produto, opcoes = {}) {
        const id = getProdutoId(produto);
        const detalheHref = `${opcoes.productPath || "pages/produtos/product.html"}?id=${encodeURIComponent(id)}`;
        const preco = getPreco(produto);
        const precoOriginal = Number(produto?.precoOriginal || produto?.PrecoOriginal || produto?.precoAntigo || produto?.PrecoAntigo || 0);
        const categoria = getCategoria(produto);
        const marca = getMarca(produto);
        const imagens = getImagens(produto, opcoes.fallbackImage || "./assets/products/camisa-over-black.png");
        const imagem = imagens[0];
        const saleClass = precoOriginal && precoOriginal > preco ? " product-sale" : "";
        const nome = produto?.nome || produto?.Nome || "Produto Zero Frame";
        const estoque = getEstoque(produto);

        const card = document.createElement(opcoes.tagName || "li");
        card.className = `product-card${saleClass}`;
        card.dataset.produtoId = id || "";

        const link = document.createElement("a");
        link.href = detalheHref;
        link.className = "product-link";

        const image = document.createElement("img");
        image.src = imagem;
        image.alt = nome;
        image.className = "product-image";
        image.onerror = () => {
            image.src = opcoes.fallbackImage || "./assets/products/camisa-over-black.png";
        };

        const info = document.createElement("div");
        info.className = "product-info";

        const category = document.createElement("p");
        category.className = "product-category";
        category.textContent = `${categoria} - ${marca}`;

        const title = document.createElement("h3");
        title.className = "product-name";
        title.textContent = nome;

        info.append(category, title);

        if (precoOriginal && precoOriginal > preco) {
            const discount = document.createElement("p");
            discount.className = "product-discount";
            discount.textContent = formatarPreco(precoOriginal);
            info.appendChild(discount);
        }

        const price = document.createElement("p");
        price.className = "product-price";
        price.textContent = formatarPreco(preco);

        const installment = document.createElement("p");
        installment.className = "price-installment";
        installment.textContent = `ou 10x de ${formatarPreco(preco / 10)} sem juros`;

        info.append(price, installment);

        if (estoque > 0) {
            const stock = document.createElement("p");
            stock.className = "product-stock";
            stock.textContent = `Estoque: ${estoque}`;
            info.appendChild(stock);
        }

        link.append(image, info);
        card.appendChild(link);

        if (imagens.length > 1) {
            const galleryButton = document.createElement("button");
            galleryButton.type = "button";
            galleryButton.className = "product-gallery-button";
            galleryButton.textContent = imagens.length === 2 ? "+ 1 imagem" : `+ ${imagens.length - 1} imagens`;
            // Reabre a galeria com a lista normalizada do proprio produto atual.
            galleryButton.addEventListener("click", (event) => {
                event.preventDefault();
                event.stopPropagation();
                abrirGaleriaProduto(imagens, nome);
            });
            card.appendChild(galleryButton);
        }

        return card;
    }

    return {
        listar,
        buscarPorId,
        listarVariacoes,
        getProdutoId,
        getCategoria,
        getMarca,
        getImagem,
        getImagens,
        getPreco,
        getEstoque,
        formatarPreco,
        renderCard,
        criarCard
    };
})();
