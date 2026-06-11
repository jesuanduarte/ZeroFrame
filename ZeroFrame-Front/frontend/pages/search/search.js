document.addEventListener("DOMContentLoaded", async () => {
    const params = new URLSearchParams(window.location.search);
    let buscaInicial = params.get("q") || "";
    const title = document.querySelector(".search-title");
    const results = document.querySelector(".search-results.products-container");
    const filtros = document.querySelector(".search-filters");

    await carregarCategoriasFiltro(params.get("Categoria"));
    aplicarFiltrosDaUrl(params);
    if (title) title.textContent = getTituloBusca(buscaInicial);

    carregarResultados(buscaInicial);

    if (filtros) {
        filtros.addEventListener("change", () => carregarResultados(buscaInicial, { atualizarUrl: true }));
    }

    document.querySelectorAll('a[href*="search.html?q=Originais"], a[href*="search.html?q=Multimarcas"]').forEach((link) => {
        link.addEventListener("click", (event) => {
            event.preventDefault();
            const url = new URL(link.href, window.location.href);
            buscaInicial = url.searchParams.get("q") || "";
            if (title) title.textContent = getTituloBusca(buscaInicial);
            carregarResultados(buscaInicial, { atualizarUrl: true });
        });
    });

    async function carregarResultados(busca, opcoes = {}) {
        if (!results) return;

        try {
            ZeroFrameApi.mostrarCarregando(results, "Buscando produtos...");
            const filtrosAtuais = montarFiltros(busca);
            if (opcoes.atualizarUrl) atualizarQueryString(filtrosAtuais);

            const produtos = await ProdutoService.listar(filtrosAtuais);
            if (!produtos.length) {
                ZeroFrameApi.mostrarMensagem(results, "Nenhum produto encontrado para essa busca.");
                return;
            }

            results.textContent = "";
            produtos.forEach((produto) => {
                results.appendChild(ProdutoService.criarCard(produto, {
                    productPath: "../../pages/produtos/product.html",
                    fallbackImage: "../../assets/products/camisa-over-black.png"
                }));
            });
        } catch (error) {
            ZeroFrameApi.mostrarMensagem(results, ZeroFrameApi.tratarErro(error, "Erro ao buscar produtos."));
        }
    }
});

function montarFiltros(busca) {
    const filtros = {};
    const checked = Array.from(document.querySelectorAll(".search-filters input:checked"));

    // Originais e Multimarcas filtram pela origem do produto, sem sair da pagina de busca.
    const origem = normalizarOrigem(busca);
    if (origem) {
        filtros.Origem = origem;
    } else if (busca) {
        filtros.Busca = busca;
    }

    checked.forEach((input) => {
        const valor = input.value || input.nextElementSibling?.textContent?.trim();
        if (!valor) return;

        if (input.id === "preco1") {
            filtros.PrecoMax = 100;
        } else if (input.id === "preco2") {
            filtros.PrecoMin = 100;
            filtros.PrecoMax = 200;
        } else if (input.id === "preco3") {
            filtros.PrecoMin = 200;
            filtros.PrecoMax = 300;
        } else if (input.id === "preco4") {
            filtros.PrecoMin = 300;
        } else if (["masculino", "feminino", "unisex"].includes(input.id)) {
            filtros.Genero = normalizarGenero(valor);
        } else if (["tam-p", "tam-m", "tam-g", "tam-gg"].includes(input.id)) {
            filtros.Tamanho = valor;
        } else if (input.id.startsWith("cor-")) {
            filtros.Cor = valor;
        } else if (input.dataset.filterParam === "Categoria") {
            filtros.Categoria = valor;
        }
    });

    return filtros;
}

async function carregarCategoriasFiltro(categoriaSelecionada = "") {
    const container = document.getElementById("categoryFilterOptions");
    if (!container) return;

    try {
        const data = await ZeroFrameApi.request("/api/categorias");
        const categorias = ZeroFrameApi.normalizarLista(data)
            .map((categoria) => ({
                id: categoria?.id || categoria?.Id,
                nome: categoria?.nome || categoria?.Nome
            }))
            .filter((categoria) => categoria.nome);

        container.textContent = "";

        if (!categorias.length) {
            const item = document.createElement("li");
            item.className = "filter-loading";
            item.textContent = "Nenhuma categoria cadastrada.";
            container.appendChild(item);
            return;
        }

        const categoriaSelecionadaNormalizada = normalizarTextoComparacao(categoriaSelecionada);

        // Categorias sincronizadas com a API; nao ha mais valores fixos no filtro.
        categorias.forEach((categoria) => {
            const id = `cat-api-${categoria.id || normalizarIdCategoria(categoria.nome)}`;
            const item = document.createElement("li");
            const input = document.createElement("input");
            const label = document.createElement("label");

            input.type = "checkbox";
            input.id = id;
            input.value = categoria.nome;
            input.dataset.filterParam = "Categoria";
            input.checked = normalizarTextoComparacao(categoria.nome) === categoriaSelecionadaNormalizada;

            label.htmlFor = id;
            label.textContent = categoria.nome;

            item.append(input, label);
            container.appendChild(item);
        });
    } catch (error) {
        console.warn("Categorias indisponiveis no filtro de busca.", error);
        container.textContent = "";
        const item = document.createElement("li");
        item.className = "filter-loading";
        item.textContent = "Categorias indisponíveis.";
        container.appendChild(item);
    }
}

function normalizarTextoComparacao(valor) {
    return String(valor || "")
        .trim()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "")
        .toLowerCase();
}

function normalizarIdCategoria(valor) {
    return normalizarTextoComparacao(valor)
        .replace(/[^a-z0-9]+/gi, "-")
        .replace(/^-+|-+$/g, "");
}

function normalizarOrigem(valor) {
    const texto = String(valor || "").trim().toLowerCase();
    if (texto === "originais" || texto === "original" || texto === "marcas originais") return "Original";
    if (texto === "multimarcas" || texto === "multimarca") return "Multimarcas";
    return "";
}

function getTituloBusca(busca) {
    const origem = normalizarOrigem(busca);
    if (origem === "Original") return "Marcas Originais";
    if (origem === "Multimarcas") return "Multimarcas";
    return busca ? `Resultados para "${busca}"` : "Produtos";
}

function normalizarGenero(valor) {
    const genero = String(valor || "").trim().toLowerCase();
    if (genero === "unisex" || genero === "unissex") return "Unissex";
    if (genero === "masculino") return "Masculino";
    if (genero === "feminino") return "Feminino";
    return valor;
}

function aplicarFiltrosDaUrl(params) {
    const mapa = {
        Genero: { Masculino: "masculino", Feminino: "feminino", Unissex: "unisex", Unisex: "unisex" },
        Tamanho: { P: "tam-p", M: "tam-m", G: "tam-g", GG: "tam-gg" },
        Cor: {
            Branco: "cor-branco",
            Preto: "cor-preto",
            Azul: "cor-azul",
            Verde: "cor-verde",
            Vermelho: "cor-vermelho",
            Amarelo: "cor-amarelo"
        }
    };

    Object.entries(mapa).forEach(([param, valores]) => {
        const valor = params.get(param);
        const id = valores[valor];
        if (id) {
            const input = document.getElementById(id);
            if (input) input.checked = true;
        }
    });

    const precoMin = params.get("PrecoMin");
    const precoMax = params.get("PrecoMax");
    const precoId = precoMax === "100" ? "preco1"
        : precoMin === "100" && precoMax === "200" ? "preco2"
        : precoMin === "200" && precoMax === "300" ? "preco3"
        : precoMin === "300" ? "preco4"
        : "";

    if (precoId) {
        const input = document.getElementById(precoId);
        if (input) input.checked = true;
    }
}

function atualizarQueryString(filtros) {
    const params = new URLSearchParams();
    Object.entries(filtros).forEach(([chave, valor]) => {
        if (valor !== undefined && valor !== null && valor !== "") {
            const chaveUrl = chave === "Origem" ? "q" : chave;
            const valorUrl = chave === "Origem" && valor === "Original" ? "Originais" : valor;
            params.set(chaveUrl, valorUrl);
        }
    });

    const query = params.toString();
    const novaUrl = `${window.location.pathname}${query ? `?${query}` : ""}`;
    window.history.replaceState({}, "", novaUrl);
}
