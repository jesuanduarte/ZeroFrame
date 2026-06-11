const ZeroFrameApi = (() => {
    const BASE_URLS = [
        "http://localhost:5140",
        "https://localhost:7291",
        "http://127.0.0.1:5140",
        "https://127.0.0.1:7291"
    ];
    const TOKEN_KEY = "zf_token";
    const USER_KEY = "zf_usuario";
    const LOGIN_MESSAGE_KEY = "zf_login_message";
    const DEV_ADMIN_FIRST_LOGIN_DONE_KEY = "ZEROFRAME_DEV_ADMIN_FIRST_LOGIN_DONE";
    const DEV_ADMIN_EMAIL = "zeroframe@gmail.com";
    const DEV_ADMIN_PASSWORD = "Admin@123";
    const MENSAGENS_POR_STATUS = {
        400: "Verifique os dados enviados.",
        401: "Você precisa estar logado.",
        403: "Você não tem permissão para acessar este recurso.",
        404: "Registro não encontrado.",
        500: "Erro interno da API."
    };

    const configuredBaseUrl = window.ZEROFRAME_API_URL || localStorage.getItem("zf_api_base_url");
    let activeBaseUrl = normalizarBaseUrl(configuredBaseUrl || BASE_URLS[0]);

    function normalizarBaseUrl(url) {
        return String(url || "").trim().replace(/\/+$/, "");
    }

    function getBaseUrls() {
        const urls = [activeBaseUrl, configuredBaseUrl, ...BASE_URLS]
            .map(normalizarBaseUrl)
            .filter(Boolean);
        return [...new Set(urls)];
    }

    function getToken() {
        return localStorage.getItem(TOKEN_KEY) || localStorage.getItem("token");
    }

    function getUsuario() {
        const raw = localStorage.getItem(USER_KEY) || localStorage.getItem("usuario");
        if (!raw) return null;

        try {
            return JSON.parse(raw);
        } catch {
            return null;
        }
    }

    function getUsuarioId() {
        const usuario = getUsuario();
        return usuario?.id || usuario?.Id || usuario?.usuarioId || usuario?.UsuarioId || localStorage.getItem("usuarioId");
    }

    function estaLogado() {
        return Boolean(getToken() && getUsuarioId());
    }

    function salvarSessao(loginResponse) {
        const token = loginResponse?.token || loginResponse?.Token || loginResponse?.accessToken || loginResponse?.jwt || loginResponse?.value?.token;
        const usuarioBase = loginResponse?.usuario || loginResponse?.user || loginResponse?.value?.usuario || loginResponse?.value || loginResponse;
        const usuario = usuarioBase && typeof usuarioBase === "object" ? {
            ...usuarioBase,
            id: usuarioBase.id || usuarioBase.Id || usuarioBase.usuarioId || usuarioBase.UsuarioId,
            usuarioId: usuarioBase.usuarioId || usuarioBase.UsuarioId || usuarioBase.id || usuarioBase.Id,
            nome: usuarioBase.nome || usuarioBase.Nome,
            email: usuarioBase.email || usuarioBase.Email,
            perfil: usuarioBase.perfil || usuarioBase.Perfil
        } : usuarioBase;

        if (token) {
            localStorage.setItem(TOKEN_KEY, token);
            localStorage.setItem("token", token);
        }

        if (usuario && typeof usuario === "object") {
            localStorage.setItem(USER_KEY, JSON.stringify(usuario));
            localStorage.setItem("usuario", JSON.stringify(usuario));
            if (usuario.id || usuario.usuarioId) {
                localStorage.setItem("usuarioId", usuario.id || usuario.usuarioId);
            }
            if (usuario.nome) localStorage.setItem("nome", usuario.nome);
            if (usuario.email) localStorage.setItem("email", usuario.email);
            if (usuario.perfil) localStorage.setItem("perfil", usuario.perfil);
        }

        return { token, usuario };
    }

    function limparSessao() {
        [TOKEN_KEY, USER_KEY, "token", "usuario", "usuarioId", "nome", "email", "perfil"].forEach((key) => {
            localStorage.removeItem(key);
            sessionStorage.removeItem(key);
        });
        // Nao remove ZEROFRAME_DEV_ADMIN_FIRST_LOGIN_DONE: logout deve impedir novo auto login admin.
    }

    function getLoginPath() {
        return window.location.pathname.includes("/pages/")
            ? "../login-page/login.html"
            : "./pages/login-page/login.html";
    }

    function redirecionarParaLogin(mensagem = "Você precisa estar logado.") {
        limparSessao();
        sessionStorage.setItem(LOGIN_MESSAGE_KEY, mensagem);
        window.location.href = getLoginPath();
    }

    function normalizarLista(data) {
        if (Array.isArray(data)) return data;
        if (Array.isArray(data?.items)) return data.items;
        if (Array.isArray(data?.Items)) return data.Items;
        if (Array.isArray(data?.value)) return data.value;
        if (Array.isArray(data?.Value)) return data.Value;
        if (Array.isArray(data?.dados)) return data.dados;
        if (Array.isArray(data?.Dados)) return data.Dados;
        if (Array.isArray(data?.data)) return data.data;
        if (Array.isArray(data?.Data)) return data.Data;
        return [];
    }

    function getMensagemErro(data, fallback) {
        if (typeof data === "string") return data;

        if (data?.errors && typeof data.errors === "object") {
            const primeiroErro = Object.values(data.errors).flat().find(Boolean);
            if (primeiroErro) return primeiroErro;
        }

        return data?.message || data?.mensagem || data?.erro || data?.title || fallback;
    }

    function normalizarMensagemErro(mensagem, status) {
        const texto = String(mensagem || "").toLowerCase();

        if (status === 0) return "Erro ao conectar com a API.";
        if (status === 401) return "Você precisa estar logado.";
        if (texto.includes("carrinho") && texto.includes("vazio")) return "Carrinho vazio.";
        if (texto.includes("estoque")) return "Produto sem estoque suficiente.";
        if (texto.includes("endereco") || texto.includes("endereço")) {
            if (texto.includes("compra") || texto.includes("finalizar")) {
                return "Cadastre um endereço antes de finalizar a compra.";
            }
        }
        if (texto.includes("failed to fetch") || texto.includes("networkerror")) return "Erro ao conectar com a API.";

        return mensagem || MENSAGENS_POR_STATUS[status] || "Não foi possível concluir a solicitação.";
    }

    function criarErroApi(mensagem, status, data) {
        const erro = new Error(normalizarMensagemErro(mensagem, status));
        erro.status = status;
        erro.data = data;
        return erro;
    }

    async function request(endpoint, options = {}) {
        const isFormData = typeof FormData !== "undefined" && options.body instanceof FormData;
        const normalizedEndpoint = normalizarEndpoint(endpoint);
        const bases = normalizedEndpoint.startsWith("http") ? [""] : getBaseUrls();
        const headers = {
            Accept: "application/json",
            ...(options.body && !isFormData ? { "Content-Type": "application/json" } : {}),
            ...options.headers
        };
        const token = getToken();

        if (token) {
            headers.Authorization = token.startsWith("Bearer ") ? token : `Bearer ${token}`;
        }

        let response;
        const body = options.body && typeof options.body !== "string" && !isFormData ? JSON.stringify(options.body) : options.body;

        for (const baseUrl of bases) {
            const url = normalizedEndpoint.startsWith("http") ? normalizedEndpoint : `${baseUrl}${normalizedEndpoint}`;

            try {
                response = await fetch(url, {
                    ...options,
                    headers,
                    body
                });

                if (baseUrl) {
                    activeBaseUrl = baseUrl;
                    localStorage.setItem("zf_api_base_url", baseUrl);
                }
                break;
            } catch {
                // Mantem o fallback entre as portas locais configuradas para o backend.
            }
        }

        if (!response) {
            throw criarErroApi("Erro ao conectar com a API.", 0, null);
        }

        const text = await response.text();
        let data = null;

        if (text) {
            try {
                data = JSON.parse(text);
            } catch {
                data = text;
            }
        }

        if (!response.ok) {
            const mensagem = getMensagemErro(data, MENSAGENS_POR_STATUS[response.status]);
            const erro = criarErroApi(mensagem, response.status, data);

            if (response.status === 401) {
                limparSessao();
            }

            throw erro;
        }

        return data;
    }

    function normalizarEndpoint(endpoint) {
        const value = String(endpoint || "").trim();
        if (!value) return "/";
        if (value.startsWith("http")) return value;

        const path = value.startsWith("/") ? value : `/${value}`;
        return path
            .replace(/^\/api\/Produto(?=\/|\?|$)/i, "/api/produtos")
            .replace(/^\/api\/Categoria(?=\/|\?|$)/i, "/api/categorias")
            .replace(/^\/api\/Pedido(?=\/|\?|$)/i, "/api/pedidos")
            .replace(/^\/api\/Usuario(?=\/|\?|$)/i, "/api/usuarios")
            .replace(/^\/Produto(?=\/|\?|$)/i, "/api/produtos")
            .replace(/^\/Categoria(?=\/|\?|$)/i, "/api/categorias")
            .replace(/^\/Pedido(?=\/|\?|$)/i, "/api/pedidos")
            .replace(/^\/Usuario(?=\/|\?|$)/i, "/api/usuarios");
    }

    function getProductImageUrl(imagePath, fallback = "") {
        const url = String(imagePath || "").trim().replace(/\\/g, "/");
        const fallbackUrl = fallback ? getProductImageUrl(fallback, "") : "";

        if (!url) return fallbackUrl;
        if (/^(javascript|data):/i.test(url)) return fallbackUrl;
        if (/^https?:\/\//i.test(url)) return url;
        if (url.startsWith("../") || url.startsWith("./")) return url;

        const apiBase = normalizarBaseUrl(activeBaseUrl);
        const semWwwroot = url.replace(/^~?\/*wwwroot\//i, "/");
        const semBarrasIniciais = semWwwroot.replace(/^\/+/, "");

        // Normaliza imagens de produtos vindas da API e evita duplicar a URL base.
        if (apiBase && semWwwroot.toLowerCase().startsWith(apiBase.toLowerCase())) {
            return semWwwroot;
        }

        if (/^(uploads|images|img|assets)\//i.test(semBarrasIniciais)) {
            return `${apiBase}/${semBarrasIniciais}`;
        }

        if (semWwwroot.startsWith("/")) {
            return `${apiBase}${semWwwroot}`;
        }

        return semWwwroot;
    }

    function obterCampoImagemProduto(produto) {
        if (!produto || typeof produto !== "object") return produto;

        return produto.imagemUrl
            || produto.ImagemUrl
            || produto.imagem
            || produto.Imagem
            || produto.urlImagem
            || produto.UrlImagem
            || "";
    }

    function resolverImagemProduto(produtoOuImagem, fallback = "") {
        return getProductImageUrl(obterCampoImagemProduto(produtoOuImagem), fallback);
    }

    function montarUrlArquivo(value, fallback) {
        return getProductImageUrl(value, fallback);
    }

    function protegerPagina() {
        if (estaLogado()) return true;

        sessionStorage.setItem(LOGIN_MESSAGE_KEY, "Você precisa estar logado.");
        window.location.href = getLoginPath();
        return false;
    }

    function mostrarMensagem(container, mensagem) {
        if (!container) return;

        container.textContent = "";
        const feedback = document.createElement("p");
        feedback.className = "api-feedback";
        feedback.textContent = normalizarMensagemErro(mensagem);
        container.appendChild(feedback);
    }

    function mostrarCarregando(container, mensagem = "Carregando...") {
        mostrarMensagem(container, mensagem);
    }

    function mostrarSucesso(container, mensagem) {
        if (!container) return;

        container.textContent = "";
        const feedback = document.createElement("p");
        feedback.className = "api-feedback api-feedback-success";
        feedback.textContent = mensagem;
        container.appendChild(feedback);
    }

    function notificar(mensagem, tipo = "erro") {
        document.querySelectorAll(".zf-toast").forEach((item) => item.remove());

        const toast = document.createElement("div");
        toast.className = `zf-toast zf-toast-${tipo}`;
        toast.textContent = normalizarMensagemErro(mensagem);
        toast.style.position = "fixed";
        toast.style.right = "1rem";
        toast.style.bottom = "1rem";
        toast.style.zIndex = "9999";
        toast.style.maxWidth = "320px";
        toast.style.padding = "0.9rem 1rem";
        toast.style.borderRadius = "0.5rem";
        toast.style.color = "#fff";
        toast.style.background = tipo === "sucesso" ? "#2e7d32" : "#b3261e";
        toast.style.boxShadow = "0 8px 24px rgba(0, 0, 0, 0.22)";

        document.body.appendChild(toast);
        setTimeout(() => toast.remove(), 4500);
    }

    function renderText(element, value, fallback = "") {
        if (element) element.textContent = value ?? fallback;
    }

    function safeImageUrl(value, fallback) {
        const url = String(value || "").trim();
        if (!url) return fallback;
        if (url.startsWith("javascript:") || url.startsWith("data:")) return fallback;
        return url;
    }

    function tratarErro(error, fallback = "Não foi possível concluir a solicitação.") {
        if (error?.status === 401) {
            redirecionarParaLogin("Você precisa estar logado.");
            return "Você precisa estar logado.";
        }

        return normalizarMensagemErro(error?.message || fallback, error?.status);
    }

    function isLocalhost() {
        return window.location.hostname === "localhost" || window.location.hostname === "127.0.0.1";
    }

    function getAdminPath() {
        return window.location.pathname.includes("/pages/")
            ? "../admin/admin.html"
            : "./pages/admin/admin.html";
    }

    async function tentarLoginAdminInicialLocal() {
        if (!isLocalhost()) return;
        if (estaLogado()) return;
        if (localStorage.getItem(DEV_ADMIN_FIRST_LOGIN_DONE_KEY) === "true") return;

        try {
            // Login automatico inicial apenas em localhost. Usa a rota real de login e nunca cria token manualmente.
            const data = await request("/api/usuarios/login", {
                method: "POST",
                body: {
                    email: DEV_ADMIN_EMAIL,
                    senha: DEV_ADMIN_PASSWORD
                }
            });

            const { usuario } = salvarSessao(data);
            localStorage.setItem(DEV_ADMIN_FIRST_LOGIN_DONE_KEY, "true");

            const perfil = usuario?.perfil || localStorage.getItem("perfil");
            if (perfil === "Administrador") {
                window.location.href = getAdminPath();
            }
        } catch {
            console.warn("Login automático inicial de administrador falhou. Verifique se a seed criou o usuário zeroframe@gmail.com.");
        }
    }

    return {
        get BASE_URL() {
            return activeBaseUrl;
        },
        BASE_URLS,
        estaLogado,
        getToken,
        getUsuario,
        getUsuarioId,
        salvarSessao,
        limparSessao,
        redirecionarParaLogin,
        normalizarLista,
        request,
        protegerPagina,
        mostrarMensagem,
        mostrarCarregando,
        mostrarSucesso,
        notificar,
        renderText,
        safeImageUrl,
        getProductImageUrl,
        obterCampoImagemProduto,
        resolverImagemProduto,
        montarUrlArquivo,
        tratarErro,
        tentarLoginAdminInicialLocal
    };
})();

window.ZeroFrameApi = ZeroFrameApi;
ZeroFrameApi.tentarLoginAdminInicialLocal();
