let pedidosGlobais = [];
let clientesGlobais = [];
let produtosGlobais = [];
let categoriasGlobais = [];
const TAMANHO_MAXIMO_IMAGEM = 5 * 1024 * 1024;
const CATEGORIAS_STORAGE_KEY = "zf_admin_categorias";
const MENSAGEM_FORMATO_IMAGEM_INVALIDO = "Formato de imagem inválido. Envie uma imagem PNG, JPG, JPEG ou WEBP.";
const ZeroFrameApi = window.ZeroFrameApi;
const Api = window.ZeroFrameApi;

document.addEventListener("DOMContentLoaded", async () => {
    if (!protegerAdmin()) return;

    initAdminMenuMobile();
    initNavigation();
    initBuscas();
    initGerenciadorTamanhos();
    initFormProduto();
    initFormPedido();
    initFormAdmin();
    initCategoriasAdmin();

    await Promise.all([
        carregarDashboard(),
        carregarCategorias(),
        carregarPedidos(),
        carregarProdutos(),
        carregarClientes()
    ]);
});

function protegerAdmin() {
    if (!Api?.estaLogado?.()) {
        Api?.redirecionarParaLogin?.("Entre como administrador para acessar o painel.");
        return false;
    }

    const usuario = Api.getUsuario();
    const perfil = usuario?.perfil || localStorage.getItem("perfil");

    if (perfil !== "Administrador") {
        Api.notificar("Acesso restrito ao administrador.");
        window.location.href = "../../index.html";
        return false;
    }

    return true;
}

function initNavigation() {
    const nav = document.querySelector(".admin-nav");
    mostrarSecaoAdmin(document.querySelector(".nav-btn.active")?.dataset.target || "dashboard", { recarregar: false });

    nav?.addEventListener("click", (event) => {
        const btn = event.target.closest(".nav-btn[data-target], .nav-btn[data-section]");
        if (!btn) return;

        event.preventDefault();
        const targetId = btn.dataset.target || btn.dataset.section;
        mostrarSecaoAdmin(targetId);
    });
}

function mostrarSecaoAdmin(targetId, opcoes = {}) {
    const section = document.getElementById(targetId);
    if (!section) return;

    document.querySelectorAll(".nav-btn[data-target], .nav-btn[data-section]").forEach((item) => {
        const itemTarget = item.dataset.target || item.dataset.section;
        item.classList.toggle("active", itemTarget === targetId);
    });

    document.querySelectorAll(".admin-section").forEach((item) => {
        item.classList.toggle("active", item.id === targetId);
    });

    if (opcoes.recarregar !== false) {
        if (targetId === "dashboard") carregarDashboard();
        if (targetId === "pedidos") carregarPedidos();
        if (targetId === "produtos") carregarProdutos();
        if (targetId === "categorias") carregarCategorias();
        if (targetId === "clientes") carregarClientes();
    }

    document.querySelector(".admin-sidebar")?.classList.remove("open");
}

window.mostrarSecaoAdmin = mostrarSecaoAdmin;

function initAdminMenuMobile() {
    const toggle = document.getElementById("adminMenuToggle");
    const sidebar = document.querySelector(".admin-sidebar");

    toggle?.addEventListener("click", () => {
        sidebar?.classList.toggle("open");
    });

    document.addEventListener("click", (event) => {
        if (!sidebar?.classList.contains("open")) return;
        if (sidebar.contains(event.target) || toggle?.contains(event.target)) return;
        sidebar.classList.remove("open");
    });
}

function initBuscas() {
    document.getElementById("search-pedido")?.addEventListener("input", (event) => {
        const termo = event.target.value.toLowerCase();
        const filtrados = pedidosGlobais.filter((pedido) => {
            const id = String(pedido.id || pedido.Id || "");
            const nome = pedido.usuario?.nome || pedido.Usuario?.Nome || "";
            return id.includes(termo) || nome.toLowerCase().includes(termo);
        });
        renderizarPedidos(filtrados);
    });

    document.getElementById("search-cliente")?.addEventListener("input", (event) => {
        const termo = event.target.value.toLowerCase();
        const filtrados = clientesGlobais.filter((cliente) => {
            const nome = cliente.nome || cliente.Nome || "";
            const email = cliente.email || cliente.Email || "";
            return nome.toLowerCase().includes(termo) || email.toLowerCase().includes(termo);
        });
        renderizarClientes(filtrados);
    });
}

async function carregarDashboard() {
    setListaMensagem("list-mais-vendidos", "Carregando...");
    setListaMensagem("list-melhores-avaliados", "Carregando...");

    try {
        const dashboard = await ZeroFrameApi.request("/api/admin/dashboard");
        atualizarDashboard(dashboard);
    } catch (error) {
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao carregar dashboard."));
        setListaMensagem("list-mais-vendidos", "Erro ao carregar.");
        setListaMensagem("list-melhores-avaliados", "Erro ao carregar.");
    }
}

function atualizarDashboard(dashboard) {
    const moeda = { style: "currency", currency: "BRL" };

    setTexto("qtd-atrasados", dashboard.pedidosAtrasados ?? dashboard.PedidosAtrasados ?? 0);
    setTexto("qtd-proximos", dashboard.pedidosProximos ?? dashboard.PedidosProximos ?? 0);
    setTexto("qtd-noprazo", dashboard.pedidosNoPrazo ?? dashboard.PedidosNoPrazo ?? 0);
    setTexto("faturamento-bruto-total", formatarMoeda(dashboard.faturamentoBrutoTotal ?? dashboard.FaturamentoBrutoTotal, moeda));
    setTexto("faturamento-bruto-mensal", formatarMoeda(dashboard.faturamentoBrutoMensal ?? dashboard.FaturamentoBrutoMensal, moeda));
    setTexto("lucro-liquido-total", formatarMoeda(dashboard.lucroLiquidoTotal ?? dashboard.LucroLiquidoTotal, moeda));
    setTexto("lucro-liquido-mensal", formatarMoeda(dashboard.lucroLiquidoMensal ?? dashboard.LucroLiquidoMensal, moeda));

    renderizarMaisVendidos(dashboard.produtosMaisVendidos || dashboard.ProdutosMaisVendidos || []);
    renderizarMelhoresAvaliados(dashboard.produtosMelhoresAvaliados || dashboard.ProdutosMelhoresAvaliados || []);
}

function renderizarMaisVendidos(produtos) {
    const lista = document.getElementById("list-mais-vendidos");
    if (!lista) return;
    lista.textContent = "";

    if (!produtos.length) {
        lista.appendChild(criarLiTexto("Sem vendas."));
    }

    produtos.forEach((produto) => {
        const li = document.createElement("li");
        const nome = document.createElement("span");
        nome.textContent = produto.nome || produto.Nome || "Produto";
        const qtd = document.createElement("strong");
        qtd.textContent = `${produto.quantidadeVendida ?? produto.QuantidadeVendida ?? 0} un.`;
        li.append(nome, qtd);
        lista.appendChild(li);
    });
}

function renderizarMelhoresAvaliados(produtos) {
    const lista = document.getElementById("list-melhores-avaliados");
    if (!lista) return;
    lista.textContent = "";

    if (!produtos.length) {
        lista.appendChild(criarLiTexto("Sem avaliações."));
        return;
    }

    produtos.forEach((produto) => {
        const li = document.createElement("li");
        const nome = document.createElement("span");
        nome.textContent = produto.nome || produto.Nome || "Produto";
        const nota = document.createElement("span");
        nota.className = "nota";
        nota.textContent = `★ ${produto.mediaAvaliacoes ?? produto.MediaAvaliacoes ?? "0.0"}`;
        li.append(nome, nota);
        lista.appendChild(li);
    });
}

function initFormProduto() {
    const imagemInput = document.getElementById("prod-imagem-arquivo");
    const limparImagemBtn = document.getElementById("prod-limpar-imagem");
    const adicionarVariacaoBtn = document.getElementById("adicionar-variacao");
    const origemSelect = document.getElementById("prod-origem");
    const vitrineSelect = document.getElementById("prod-vitrine");

    imagemInput?.addEventListener("change", () => {
        const arquivos = Array.from(imagemInput.files || []);
        if (!arquivos.length) return;

        const erro = validarImagensProduto(arquivos);
        if (erro) {
            ZeroFrameApi.notificar(erro);
            imagemInput.value = "";
            atualizarPreviewProduto([]);
            return;
        }

        atualizarPreviewProduto(arquivos.map((arquivo) => URL.createObjectURL(arquivo)));
    });

    limparImagemBtn?.addEventListener("click", () => {
        if (imagemInput) {
            imagemInput.value = "";
            delete imagemInput.dataset.imagemAtual;
        }
        atualizarPreviewProduto([]);
    });

    adicionarVariacaoBtn?.addEventListener("click", () => adicionarLinhaVariacao());
    origemSelect?.addEventListener("change", sincronizarOrigemEVitrine);
    vitrineSelect?.addEventListener("change", sincronizarOrigemEVitrine);

    document.getElementById("form-produto")?.addEventListener("submit", async (event) => {
        event.preventDefault();

        const id = document.getElementById("prod-id").value;
        const precoCusto = Number(document.getElementById("prod-preco-custo").value);
        const precoVenda = Number(document.getElementById("prod-preco").value);
        const tipoDesconto = document.getElementById("prod-tipo-desconto").value;
        const desconto = Number(document.getElementById("prod-desconto-valor").value || 0);

        if (precoVenda < precoCusto) {
            ZeroFrameApi.notificar("O preço de venda não pode ser menor que o preço de custo.");
            return;
        }

        const precoComDesconto = tipoDesconto === "porcentagem"
            ? precoVenda * (1 - desconto / 100)
            : tipoDesconto === "fixo"
                ? precoVenda - desconto
                : precoVenda;

        if (precoComDesconto < precoCusto) {
            ZeroFrameApi.notificar("O desconto reduz o produto abaixo do preço de custo.");
            return;
        }

        try {
            const body = montarProdutoFormData(id);
            const produtoSalvo = await ZeroFrameApi.request(id ? `/api/produtos/${id}` : "/api/produtos", {
                method: id ? "PUT" : "POST",
                body
            });
            const produtoId = Number(id || obterProdutoId(produtoSalvo));

            if (produtoId) {
                await salvarVariacoesProduto(produtoId, Boolean(id));
            }

            ZeroFrameApi.notificar("Produto salvo com sucesso.", "sucesso");
            fecharModal("modal-produto");
            await Promise.all([carregarProdutos(), carregarDashboard()]);
        } catch (error) {
            ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao salvar produto."));
        }
    });
}

function montarProdutoFormData(id) {
    const formData = new FormData();
    const imagemInput = document.getElementById("prod-imagem-arquivo");
    const imagens = Array.from(imagemInput?.files || []);

    if (imagens.length) {
        const erro = validarImagensProduto(imagens);
        if (erro) throw new Error(erro);
    }

    if (!id && imagens.length < 2) {
        throw new Error("Selecione pelo menos 2 imagens do produto.");
    }

    if (id) formData.append("Id", id);
    formData.append("Nome", document.getElementById("prod-nome").value.trim());
    formData.append("Descricao", document.getElementById("prod-descricao").value.trim());
    formData.append("PrecoCusto", document.getElementById("prod-preco-custo").value);
    formData.append("Preco", document.getElementById("prod-preco").value);
    formData.append("TipoDesconto", document.getElementById("prod-tipo-desconto").value);
    formData.append("Desconto", document.getElementById("prod-desconto-valor").value || "0");
    if (imagens.length) {
        // Envia todas as imagens selecionadas; a API usa a primeira como principal e preserva a galeria real do produto.
        imagens.forEach((imagem) => formData.append("ImagemArquivos", imagem));
    } else if (id && imagemInput?.dataset.imagemAtual) {
        formData.append("ImagemUrl", imagemInput.dataset.imagemAtual);
    }
    formData.append("CategoriaId", document.getElementById("prod-categoria").value);
    formData.append("Genero", document.getElementById("prod-genero").value);
    formData.append("Cor", document.getElementById("prod-cor").value.trim());
    formData.append("SecaoVitrine", document.getElementById("prod-vitrine").value);
    formData.append("Marca", document.getElementById("prod-marca").value.trim());
    formData.append("Origem", document.getElementById("prod-origem").value);
    formData.append("TipoTamanho", document.getElementById("prod-tipo-tamanho").value);
    formData.append("TamanhosDisponiveis", obterTamanhosSelecionados());
    formData.append("Ativo", "true");
    return formData;
}

function validarImagemProduto(arquivo) {
    const extensoesPermitidas = [".png", ".jpg", ".jpeg", ".webp"];
    const contentTypesPermitidos = ["image/png", "image/jpeg", "image/webp"];
    const nome = String(arquivo.name || "").toLowerCase();
    const contentType = String(arquivo.type || "").toLowerCase();
    const extensaoValida = extensoesPermitidas.some((extensao) => nome.endsWith(extensao));
    const contentTypeValido = !contentType || contentTypesPermitidos.includes(contentType);

    if (!extensaoValida || !contentTypeValido) {
        return MENSAGEM_FORMATO_IMAGEM_INVALIDO;
    }

    if (arquivo.size > TAMANHO_MAXIMO_IMAGEM) {
        return "A imagem deve ter no máximo 5MB.";
    }

    return "";
}

function validarImagensProduto(arquivos) {
    if (arquivos.length > 5) {
        return "Selecione no máximo 5 imagens.";
    }

    const invalida = arquivos.find((arquivo) => validarImagemProduto(arquivo) === MENSAGEM_FORMATO_IMAGEM_INVALIDO);
    if (invalida) {
        return MENSAGEM_FORMATO_IMAGEM_INVALIDO;
    }

    const muitoGrande = arquivos.find((arquivo) => arquivo.size > TAMANHO_MAXIMO_IMAGEM);
    if (muitoGrande) {
        return "Cada imagem deve ter no máximo 5MB.";
    }

    return "";
}

function atualizarPreviewProduto(srcs) {
    const wrapper = document.getElementById("prod-imagens-preview");
    if (!wrapper) return;

    const limparBtn = document.getElementById("prod-limpar-imagem");
    wrapper.textContent = "";

    const listaSrcs = Array.isArray(srcs) ? srcs : [srcs].filter(Boolean);
    const imagens = listaSrcs.length ? listaSrcs : ["../../assets/logo zf.png"];
    imagens.slice(0, 5).forEach((src, index) => {
        const preview = document.createElement("img");
        preview.className = "prod-img-preview";
        preview.src = src;
        preview.alt = `Preview da imagem ${index + 1}`;
        wrapper.appendChild(preview);
    });

    if (limparBtn) wrapper.appendChild(limparBtn);
}

function obterImagemProdutoAdmin(produto) {
    if (typeof ProdutoService !== "undefined" && ProdutoService.getImagem) {
        return ProdutoService.getImagem(produto, "../../assets/logo zf.png");
    }

    return ZeroFrameApi.resolverImagemProduto(produto, "../../assets/logo zf.png");
}

function obterImagemOriginalProdutoAdmin(produto) {
    return ZeroFrameApi.obterCampoImagemProduto(produto);
}

function obterProdutoId(produto) {
    return Number(produto?.id ?? produto?.Id ?? produto?.produtoId ?? produto?.ProdutoId ?? 0);
}

function obterCategoriaId(categoria) {
    return Number(categoria?.id ?? categoria?.Id ?? categoria?.categoriaId ?? categoria?.CategoriaId ?? 0);
}

function obterClienteId(cliente) {
    return Number(cliente?.id ?? cliente?.Id ?? cliente?.usuarioId ?? cliente?.UsuarioId ?? 0);
}

function calcularEstoqueProduto(produto) {
    const variacoes = produto?.variacoes || produto?.Variacoes || [];
    if (!Array.isArray(variacoes) || !variacoes.length) return 0;
    return variacoes.reduce((total, variacao) => total + Number(variacao.estoque ?? variacao.Estoque ?? 0), 0);
}

function sincronizarOrigemEVitrine(event) {
    const origemSelect = document.getElementById("prod-origem");
    const vitrineSelect = document.getElementById("prod-vitrine");
    if (!origemSelect || !vitrineSelect) return;

    if (event?.target === vitrineSelect) {
        if (vitrineSelect.value === "Originais") origemSelect.value = "Original";
        if (vitrineSelect.value === "Multimarcas") origemSelect.value = "Multimarcas";
        return;
    }

    if (origemSelect.value === "Original" && vitrineSelect.value === "Multimarcas") {
        vitrineSelect.value = "Originais";
    }

    if (origemSelect.value === "Multimarcas" && vitrineSelect.value === "Originais") {
        vitrineSelect.value = "Multimarcas";
    }
}

function adicionarLinhaVariacao(variacao = {}) {
    const container = document.getElementById("variacoes-container");
    if (!container) return;

    const row = document.createElement("div");
    row.className = "variation-row";
    row.dataset.variacaoId = variacao.id || variacao.Id || "";

    const tamanhoLabel = criarCampoVariacao("Tamanho", "text", "variacao-tamanho", variacao.tamanho || variacao.Tamanho || "");
    const corLabel = criarCampoVariacao("Cor", "text", "variacao-cor", variacao.cor || variacao.Cor || "");
    const estoqueLabel = criarCampoVariacao("Estoque", "number", "variacao-estoque", variacao.estoque ?? variacao.Estoque ?? 0);
    estoqueLabel.querySelector("input").min = "0";

    const remover = criarBotao("Remover", "btn-delete", () => {
        row.remove();
        if (!container.children.length) adicionarLinhaVariacao();
    });

    row.append(tamanhoLabel, corLabel, estoqueLabel, remover);
    container.appendChild(row);
}

function criarCampoVariacao(labelText, type, className, value) {
    const label = document.createElement("label");
    label.textContent = labelText;
    const input = document.createElement("input");
    input.type = type;
    input.className = className;
    input.value = value;
    input.required = true;
    label.appendChild(input);
    return label;
}

function obterVariacoesFormulario() {
    return Array.from(document.querySelectorAll(".variation-row")).map((row) => ({
        id: row.dataset.variacaoId,
        tamanho: row.querySelector(".variacao-tamanho")?.value.trim() || "",
        cor: row.querySelector(".variacao-cor")?.value.trim() || "",
        estoque: Number(row.querySelector(".variacao-estoque")?.value || 0)
    })).filter((variacao) => variacao.tamanho && variacao.cor);
}

async function salvarVariacoesProduto(produtoId, editando) {
    const variacoes = obterVariacoesFormulario();

    if (!variacoes.length) {
        throw new Error("Cadastre pelo menos uma variação com tamanho, cor e estoque.");
    }

    if (editando) {
        const produtoAtual = produtosGlobais.find((produto) => Number(produto.id || produto.Id) === Number(produtoId));
        const variacoesAtuais = produtoAtual?.variacoes || produtoAtual?.Variacoes || [];

        await Promise.all(variacoesAtuais.map((variacao) => {
            const variacaoId = variacao.id || variacao.Id;
            return variacaoId
                ? ZeroFrameApi.request(`/api/produtos/${produtoId}/variacoes/${variacaoId}`, { method: "DELETE" }).catch(() => null)
                : Promise.resolve();
        }));
    }

    for (const variacao of variacoes) {
        await ZeroFrameApi.request(`/api/produtos/${produtoId}/variacoes`, {
            method: "POST",
            body: {
                tamanho: variacao.tamanho,
                cor: variacao.cor,
                estoque: variacao.estoque
            }
        });
    }
}

function obterTamanhosSelecionados() {
    const tipo = document.getElementById("prod-tipo-tamanho").value;
    if (tipo === "Unico") return "Único";

    const gradeId = tipo === "Calcado" ? "grade-calcado" : "grade-roupa";
    return Array.from(document.querySelectorAll(`#${gradeId} input:checked`))
        .map((input) => input.value)
        .join(",");
}

async function carregarProdutos() {
    const tbody = document.getElementById("produtos-tbody");
    if (tbody) setTabelaMensagem(tbody, "Carregando produtos...", 7);

    try {
        const data = await ZeroFrameApi.request("/api/produtos/admin/todos?PageSize=50");
        produtosGlobais = ZeroFrameApi.normalizarLista(data);
        renderizarProdutos(produtosGlobais);
    } catch (error) {
        if (tbody) setTabelaMensagem(tbody, ZeroFrameApi.tratarErro(error, "Erro ao carregar produtos."), 7);
    }
}

function renderizarProdutos(produtos) {
    const tbody = document.getElementById("produtos-tbody");
    if (!tbody) return;
    tbody.textContent = "";

    if (!produtos.length) {
        setTabelaMensagem(tbody, "Nenhum produto cadastrado.", 7);
        return;
    }

    produtos.forEach((produto) => {
        const produtoId = obterProdutoId(produto);
        const tr = document.createElement("tr");
        tr.dataset.produtoId = String(produtoId);
        const desconto = produto.desconto ?? produto.Desconto ?? 0;
        const tipoDesconto = produto.tipoDesconto || produto.TipoDesconto || "nenhum";

        const imgTd = document.createElement("td");
        const img = document.createElement("img");
        img.src = obterImagemProdutoAdmin(produto);
        img.className = "prod-img-min";
        img.alt = produto.nome || produto.Nome || "Produto";
        img.onerror = () => {
            img.src = "../../assets/logo zf.png";
        };
        imgTd.appendChild(img);

        tr.append(
            imgTd,
            criarTd(produto.nome || produto.Nome || ""),
            criarTd(formatarMoeda(produto.precoCusto ?? produto.PrecoCusto)),
            criarTd(formatarMoeda(produto.preco ?? produto.Preco)),
            criarTd(desconto ? `${tipoDesconto === "porcentagem" ? `${desconto}%` : formatarMoeda(desconto)}` : "Nenhum"),
            criarTd(calcularEstoqueProduto(produto)),
            criarTdAcoes([
                criarBotao("Editar", "btn-edit", () => abrirModalProduto(produtoId)),
                // O botao usa o ID normalizado do produto renderizado nesta linha para nao remover outro item.
                criarBotao("Remover", "btn-delete", () => removerProduto(produtoId))
            ])
        );

        tbody.appendChild(tr);
    });
}

window.abrirModalProduto = function abrirModalProduto(produtoId = null) {
    document.getElementById("form-produto")?.reset();
    setValor("prod-id", "");
    const imagemInput = document.getElementById("prod-imagem-arquivo");
    if (imagemInput) {
        imagemInput.value = "";
        delete imagemInput.dataset.imagemAtual;
    }
    atualizarPreviewProduto([]);
    const variacoesContainer = document.getElementById("variacoes-container");
    if (variacoesContainer) {
        variacoesContainer.textContent = "";
        adicionarLinhaVariacao();
    }
    document.querySelectorAll(".grade-tamanho").forEach((el) => { el.style.display = "none"; });
    document.querySelectorAll(".grade-tamanho input[type='checkbox']").forEach((input) => { input.checked = false; });
    setTexto("modal-produto-titulo", "Adicionar Produto");

    if (produtoId) {
        const produto = produtosGlobais.find((item) => obterProdutoId(item) === Number(produtoId));
        if (!produto) return;

        setTexto("modal-produto-titulo", "Editar Produto");
        setValor("prod-id", produto.id || produto.Id || "");
        setValor("prod-nome", produto.nome || produto.Nome || "");
        setValor("prod-descricao", produto.descricao || produto.Descricao || "");
        setValor("prod-preco-custo", produto.precoCusto ?? produto.PrecoCusto ?? "");
        setValor("prod-preco", produto.preco ?? produto.Preco ?? "");
        setValor("prod-tipo-desconto", produto.tipoDesconto || produto.TipoDesconto || "porcentagem");
        setValor("prod-desconto-valor", produto.desconto ?? produto.Desconto ?? 0);
        const imagemAtual = obterImagemOriginalProdutoAdmin(produto);
        if (imagemInput && imagemAtual) {
            imagemInput.dataset.imagemAtual = imagemAtual;
        }
        atualizarPreviewProduto([obterImagemProdutoAdmin(produto)]);
        setValor("prod-marca", produto.marca || produto.Marca || "");
        setValor("prod-origem", produto.origem || produto.Origem || "Original");
        setValor("prod-categoria", produto.categoriaId || produto.CategoriaId || "");
        setValor("prod-genero", produto.genero || produto.Genero || "Unissex");
        setValor("prod-cor", produto.cor || produto.Cor || "");
        setValor("prod-vitrine", produto.secaoVitrine || produto.SecaoVitrine || "Nenhum");
        setValor("prod-tipo-tamanho", produto.tipoTamanho || produto.TipoTamanho || "Unico");
        document.getElementById("prod-tipo-tamanho")?.dispatchEvent(new Event("change"));

        const tamanhos = String(produto.tamanhosDisponiveis || produto.TamanhosDisponiveis || "").split(",");
        document.querySelectorAll(".grade-tamanho input[type='checkbox']").forEach((input) => {
            input.checked = tamanhos.includes(input.value);
        });

        const variacoes = produto.variacoes || produto.Variacoes || [];
        if (variacoesContainer) {
            variacoesContainer.textContent = "";
            if (variacoes.length) {
                variacoes.forEach((variacao) => adicionarLinhaVariacao(variacao));
            } else {
                adicionarLinhaVariacao();
            }
        }
    }

    document.getElementById("modal-produto")?.classList.add("open");
};

window.removerProduto = async function removerProduto(produtoId) {
    const id = Number(produtoId);
    const produto = produtosGlobais.find((item) => obterProdutoId(item) === id);
    const nome = produto?.nome || produto?.Nome || "este produto";

    if (!id || !confirm(`Remover "${nome}"?`)) return;

    try {
        await ZeroFrameApi.request(`/api/produtos/${id}`, { method: "DELETE" });
        // Remove somente o produto confirmado da lista em memoria antes de recarregar a tabela.
        produtosGlobais = produtosGlobais.filter((item) => obterProdutoId(item) !== id);
        renderizarProdutos(produtosGlobais);
        ZeroFrameApi.notificar("Produto removido com sucesso.", "sucesso");
        await Promise.all([carregarProdutos(), carregarDashboard()]);
    } catch (error) {
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao remover produto."));
    }
};

async function carregarPedidos() {
    const tbody = document.getElementById("pedidos-tbody");
    if (tbody) setTabelaMensagem(tbody, "Carregando pedidos...", 6);

    try {
        const data = await ZeroFrameApi.request("/api/pedidos?PageSize=50");
        pedidosGlobais = ZeroFrameApi.normalizarLista(data);
        renderizarPedidos(pedidosGlobais);
    } catch (error) {
        if (tbody) setTabelaMensagem(tbody, ZeroFrameApi.tratarErro(error, "Erro ao carregar pedidos."), 6);
    }
}

function renderizarPedidos(pedidos) {
    const tbody = document.getElementById("pedidos-tbody");
    if (!tbody) return;
    tbody.textContent = "";

    if (!pedidos.length) {
        setTabelaMensagem(tbody, "Nenhum pedido encontrado.", 6);
        return;
    }

    pedidos.forEach((pedido) => {
        const id = pedido.id || pedido.Id;
        const usuario = pedido.usuario || pedido.Usuario || {};
        const tr = document.createElement("tr");
        tr.append(
            criarTd(`#${id}`),
            criarTd(usuario.nome || usuario.Nome || "Desconhecido"),
            criarTd(formatarData(pedido.previsaoEntrega || pedido.PrevisaoEntrega)),
            criarTd(pedido.statusEntrega || pedido.StatusEntrega || "Pendente"),
            criarTd(formatarMoeda(pedido.valorTotalComFrete ?? pedido.ValorTotalComFrete ?? pedido.valorTotal ?? pedido.ValorTotal)),
            criarTdAcoes([criarBotao("Alterar", "btn-edit", () => abrirModalPedido(id))])
        );
        tbody.appendChild(tr);
    });
}

window.abrirModalPedido = function abrirModalPedido(pedidoId) {
    const pedido = pedidosGlobais.find((item) => (item.id || item.Id) === pedidoId);
    if (!pedido) return;

    setTexto("edit-pedido-id", pedidoId);
    setValor("edit-pedido-status", pedido.statusEntrega || pedido.StatusEntrega || "Pendente");
    setValor("edit-pedido-previsao", formatarDataInput(pedido.previsaoEntrega || pedido.PrevisaoEntrega));
    document.getElementById("modal-pedido")?.classList.add("open");
};

function initFormPedido() {
    document.getElementById("form-pedido")?.addEventListener("submit", async (event) => {
        event.preventDefault();
        const pedidoId = document.getElementById("edit-pedido-id")?.textContent;
        const statusEntrega = document.getElementById("edit-pedido-status")?.value;
        const previsaoEntrega = document.getElementById("edit-pedido-previsao")?.value || null;

        try {
            await ZeroFrameApi.request(`/api/pedidos/${pedidoId}/status-entrega`, {
                method: "PUT",
                body: { statusEntrega, previsaoEntrega }
            });
            ZeroFrameApi.notificar("Pedido atualizado com sucesso.", "sucesso");
            fecharModal("modal-pedido");
            await Promise.all([carregarPedidos(), carregarDashboard()]);
        } catch (error) {
            ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao atualizar pedido."));
        }
    });
}

async function carregarCategorias() {
    const tbody = document.getElementById("categorias-tbody");
    if (tbody) setTabelaMensagem(tbody, "Carregando categorias...", 4);

    try {
        const data = await ZeroFrameApi.request("/api/categorias");
        categoriasGlobais = ZeroFrameApi.normalizarLista(data);
    } catch (error) {
        categoriasGlobais = [];
        console.warn("Falha ao carregar categorias na API.", error);
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao carregar categorias."));
        preencherSelectCategorias();
        renderizarCategorias();
        return;
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "API indisponível. Categorias carregadas do armazenamento local."));
    }

    preencherSelectCategorias();
    renderizarCategorias();
}

function initCategoriasAdmin() {
    document.getElementById("form-categoria")?.addEventListener("submit", salvarCategoria);
    document.getElementById("limpar-categoria")?.addEventListener("click", limparFormularioCategoria);

    document.getElementById("categorias-tbody")?.addEventListener("click", async (event) => {
        const editar = event.target.closest("[data-editar-categoria]");
        const excluir = event.target.closest("[data-excluir-categoria]");

        if (editar) {
            preencherFormularioCategoria(Number(editar.dataset.editarCategoria));
        }

        if (excluir) {
            await excluirCategoria(Number(excluir.dataset.excluirCategoria));
        }
    });
}

function preencherSelectCategorias() {
    const select = document.getElementById("prod-categoria");
    if (!select) return;

    select.textContent = "";
    categoriasGlobais.forEach((categoria) => {
        const categoriaId = obterCategoriaId(categoria);
        const option = document.createElement("option");
        option.value = categoriaId;
        option.textContent = categoria.nome || categoria.Nome || "Categoria";
        select.appendChild(option);
    });
}

async function salvarCategoria(event) {
    event.preventDefault();

    const id = Number(document.getElementById("categoria-id").value);
    const categoria = {
        nome: document.getElementById("categoria-nome").value.trim(),
        descricao: document.getElementById("categoria-descricao").value.trim()
    };

    if (!categoria.nome) {
        ZeroFrameApi.notificar("Informe o nome da categoria.");
        return;
    }

    try {
        if (id) {
            await ZeroFrameApi.request(`/api/categorias/${id}`, {
                method: "PUT",
                body: { id, ...categoria }
            });
        } else {
            await ZeroFrameApi.request("/api/categorias", {
                method: "POST",
                body: categoria
            });
        }

        ZeroFrameApi.notificar("Categoria salva com sucesso.", "sucesso");
        limparFormularioCategoria();
        await carregarCategorias();
    } catch (error) {
        console.warn("Falha ao salvar categoria na API.", error);
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao salvar categoria."));
        return;
        ZeroFrameApi.notificar("Categoria salva localmente. A API não confirmou a operação.", "sucesso");
        limparFormularioCategoria();
        await carregarCategorias();
    }
}

function renderizarCategorias() {
    const tbody = document.getElementById("categorias-tbody");
    const info = document.getElementById("categorias-pagination-info");
    if (!tbody) return;

    tbody.textContent = "";

    if (!categoriasGlobais.length) {
        setTabelaMensagem(tbody, "Nenhuma categoria cadastrada.", 4);
        if (info) info.textContent = "Mostrando 0 a 0 de 0 categorias";
        return;
    }

    categoriasGlobais.slice(0, 5).forEach((categoria) => {
        const id = obterCategoriaId(categoria);
        const tr = document.createElement("tr");
        tr.dataset.categoriaId = String(id);
        tr.append(
            criarTd(id),
            criarTd(categoria.nome || categoria.Nome || ""),
            criarTd(categoria.descricao || categoria.Descricao || ""),
            criarTdAcoes([
                criarBotaoAcao("Editar", "btn-edit", "data-editar-categoria", id),
                criarBotaoAcao("Excluir", "btn-delete", "data-excluir-categoria", id)
            ])
        );
        tbody.appendChild(tr);
    });

    if (info) {
        const exibidas = Math.min(5, categoriasGlobais.length);
        info.textContent = `Mostrando 1 a ${exibidas} de ${categoriasGlobais.length} categorias`;
    }
}

function preencherFormularioCategoria(id) {
    const categoria = categoriasGlobais.find((item) => obterCategoriaId(item) === Number(id));
    if (!categoria) {
        ZeroFrameApi.notificar("Categoria nao encontrada na lista atual.");
        return;
    }

    // Carrega os dados da categoria selecionada no formulario para salvar via PUT.
    setValor("categoria-id", id);
    setValor("categoria-nome", categoria.nome || categoria.Nome || "");
    setValor("categoria-descricao", categoria.descricao || categoria.Descricao || "");
    document.getElementById("categoria-nome")?.focus();
}

async function excluirCategoria(id) {
    if (!id || !confirm("Excluir esta categoria?")) return;

    try {
        await ZeroFrameApi.request(`/api/categorias/${id}`, { method: "DELETE" });
        ZeroFrameApi.notificar("Categoria excluída com sucesso.", "sucesso");
        await carregarCategorias();
    } catch (error) {
        console.warn("Falha ao excluir categoria na API.", error);
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao excluir categoria."));
        return;
        categoriasGlobais = categoriasGlobais.filter((categoria) => Number(categoria.id || categoria.Id) !== id);
        salvarCategoriasLocal(categoriasGlobais);
        ZeroFrameApi.notificar("Categoria removida localmente. A API não confirmou a operação.", "sucesso");
        renderizarCategorias();
        preencherSelectCategorias();
    }
}

function limparFormularioCategoria() {
    setValor("categoria-id", "");
    setValor("categoria-nome", "");
    setValor("categoria-descricao", "");
}

function salvarCategoriaLocal(id, categoria) {
    const categorias = carregarCategoriasLocal();

    if (id) {
        const index = categorias.findIndex((item) => Number(item.id || item.Id) === id);
        if (index >= 0) {
            categorias[index] = { ...categorias[index], ...categoria, id };
        }
    } else {
        const novoId = Math.max(0, ...categorias.map((item) => Number(item.id || item.Id || 0))) + 1;
        categorias.push({ id: novoId, ...categoria });
    }

    salvarCategoriasLocal(categorias);
    categoriasGlobais = categorias;
}

function carregarCategoriasLocal() {
    try {
        const categorias = JSON.parse(localStorage.getItem(CATEGORIAS_STORAGE_KEY));
        if (Array.isArray(categorias) && categorias.length) return categorias;
    } catch {
        // Usa os exemplos abaixo quando ainda nao houver dados locais.
    }

    return [
        { id: 1, nome: "Eletrônicos", descricao: "Produtos eletrônicos em geral" },
        { id: 2, nome: "Informática", descricao: "Computadores, acessórios e periféricos" },
        { id: 3, nome: "Celulares", descricao: "Smartphones e acessórios" },
        { id: 4, nome: "Eletrodomésticos", descricao: "Eletrodomésticos para sua casa" },
        { id: 5, nome: "Casa e Decoração", descricao: "Itens para casa e decoração" }
    ];
}

function salvarCategoriasLocal(categorias) {
    localStorage.setItem(CATEGORIAS_STORAGE_KEY, JSON.stringify(categorias));
}

function criarBotaoAcao(texto, classe, dataAttribute, id) {
    const button = criarBotao(texto, classe, () => {});
    button.setAttribute(dataAttribute, id);
    return button;
}

async function carregarClientes() {
    const tbody = document.getElementById("clientes-tbody");
    if (tbody) setTabelaMensagem(tbody, "Carregando clientes...", 5);

    try {
        const data = await ZeroFrameApi.request("/api/usuarios/todos?PageSize=50");
        clientesGlobais = ZeroFrameApi.normalizarLista(data);
        renderizarClientes(clientesGlobais);
    } catch (error) {
        if (tbody) setTabelaMensagem(tbody, ZeroFrameApi.tratarErro(error, "Erro ao carregar clientes."), 5);
    }
}

function renderizarClientes(clientes) {
    const tbody = document.getElementById("clientes-tbody");
    if (!tbody) return;
    tbody.textContent = "";

    if (!clientes.length) {
        setTabelaMensagem(tbody, "Nenhum cliente encontrado.", 5);
        return;
    }

    clientes.forEach((cliente) => {
        const id = obterClienteId(cliente);
        const perfil = cliente.perfil || cliente.Perfil || "";
        const tr = document.createElement("tr");
        tr.dataset.clienteId = String(id);
        tr.append(
            criarTd(`#${id}`),
            criarTd(cliente.nome || cliente.Nome || ""),
            criarTd(cliente.email || cliente.Email || ""),
            criarTd(cliente.quantidadePedidos ?? cliente.QuantidadePedidos ?? pedidosGlobais.filter((pedido) => (pedido.usuarioId || pedido.UsuarioId) === id).length),
            criarTdAcoes([
                criarBotao("Ver Compras", "btn-edit", () => filtrarPedidosPorCliente(id)),
                // Exclui somente o cliente desta linha; administradores sao bloqueados em removerCliente.
                criarBotao("Excluir", "btn-delete", () => removerCliente(id, perfil))
            ])
        );
        tbody.appendChild(tr);
    });
}

window.removerCliente = async function removerCliente(clienteId, perfil = "") {
    const id = Number(clienteId);
    const cliente = clientesGlobais.find((item) => obterClienteId(item) === id);
    const perfilCliente = perfil || cliente?.perfil || cliente?.Perfil || "";

    if (String(perfilCliente).toLowerCase() === "administrador") {
        ZeroFrameApi.notificar("Administradores não podem ser removidos.");
        return;
    }

    if (!id || !confirm("Tem certeza que deseja excluir este cliente?")) return;

    try {
        await ZeroFrameApi.request(`/api/usuarios/${id}`, { method: "DELETE" });
        clientesGlobais = clientesGlobais.filter((item) => obterClienteId(item) !== id);
        renderizarClientes(clientesGlobais);
        ZeroFrameApi.notificar("Cliente removido com sucesso.", "sucesso");
    } catch (error) {
        ZeroFrameApi.notificar(ZeroFrameApi.tratarErro(error, "Erro ao remover cliente."));
    }
};

window.filtrarPedidosPorCliente = function filtrarPedidosPorCliente(id) {
    document.querySelector('[data-target="pedidos"]')?.click();
    renderizarPedidos(pedidosGlobais.filter((pedido) => (pedido.usuarioId || pedido.UsuarioId) === id));
};

window.filtrarPedidosDashboard = function filtrarPedidosDashboard(filtro) {
    document.querySelector('[data-target="pedidos"]')?.click();
    const hoje = new Date();
    renderizarPedidos(pedidosGlobais.filter((pedido) => {
        const status = pedido.statusEntrega || pedido.StatusEntrega || "";
        const previsao = pedido.previsaoEntrega || pedido.PrevisaoEntrega;
        if (status === "Entregue" || status === "Cancelado") return false;
        if (!previsao) return filtro === "noprazo";
        const diff = Math.ceil((new Date(previsao) - hoje) / (1000 * 3600 * 24));
        if (filtro === "atrasados") return diff < 0;
        if (filtro === "proximos") return diff >= 0 && diff <= 3;
        return diff > 3;
    }));
};

function initGerenciadorTamanhos() {
    const grid = document.querySelector(".calcados-grid");
    if (grid && !grid.children.length) {
        for (let i = 33; i <= 46; i += 1) {
            const label = document.createElement("label");
            const input = document.createElement("input");
            input.type = "checkbox";
            input.value = String(i);
            label.append(input, ` ${i}`);
            grid.appendChild(label);
        }
    }

    document.getElementById("prod-tipo-tamanho")?.addEventListener("change", (event) => {
        document.querySelectorAll(".grade-tamanho").forEach((el) => { el.style.display = "none"; });
        if (event.target.value === "Roupa") document.getElementById("grade-roupa").style.display = "block";
        if (event.target.value === "Calcado") document.getElementById("grade-calcado").style.display = "block";
    });
}

function initFormAdmin() {
    document.getElementById("form-admin")?.addEventListener("submit", (event) => {
        event.preventDefault();
        ZeroFrameApi.notificar("A API atual não possui endpoint para criar administrador pelo painel.");
    });
}

window.abrirModalCriarAdmin = function abrirModalCriarAdmin() {
    document.getElementById("form-admin")?.reset();
    document.getElementById("modal-admin")?.classList.add("open");
};

window.fecharModal = function fecharModal(id) {
    document.getElementById(id)?.classList.remove("open");
};

function criarTd(texto) {
    const td = document.createElement("td");
    td.textContent = texto ?? "";
    return td;
}

function criarTdAcoes(botoes) {
    const td = document.createElement("td");
    botoes.forEach((botao) => td.appendChild(botao));
    return td;
}

function criarBotao(texto, classe, onClick) {
    const button = document.createElement("button");
    button.type = "button";
    button.className = classe;
    button.textContent = texto;
    button.addEventListener("click", onClick);
    return button;
}

function criarLiTexto(texto) {
    const li = document.createElement("li");
    li.textContent = texto;
    return li;
}

function setTabelaMensagem(tbody, mensagem, colSpan) {
    tbody.textContent = "";
    const tr = document.createElement("tr");
    const td = document.createElement("td");
    td.colSpan = colSpan;
    td.textContent = mensagem;
    tr.appendChild(td);
    tbody.appendChild(tr);
}

function setListaMensagem(id, mensagem) {
    const lista = document.getElementById(id);
    if (!lista) return;
    lista.textContent = "";
    lista.appendChild(criarLiTexto(mensagem));
}

function setTexto(id, valor) {
    const element = document.getElementById(id);
    if (element) element.textContent = valor;
}

function setValor(id, valor) {
    const element = document.getElementById(id);
    if (element) element.value = valor;
}

function formatarMoeda(valor) {
    return Number(valor || 0).toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function formatarData(data) {
    if (!data) return "Sem previsão";
    return new Date(data).toLocaleDateString("pt-BR");
}

function formatarDataInput(data) {
    if (!data) return "";
    return new Date(data).toISOString().slice(0, 10);
}
