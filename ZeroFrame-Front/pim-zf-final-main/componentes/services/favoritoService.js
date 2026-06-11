const FavoritoService = (() => {
    const STORAGE_KEY = "zf_favoritos";

    function listarLocal() {
        try {
            return JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
        } catch {
            return [];
        }
    }

    function salvarLocal(favoritos) {
        localStorage.setItem(STORAGE_KEY, JSON.stringify(favoritos));
    }

    async function listar() {
        const usuarioId = ZeroFrameApi.getUsuarioId();
        if (!usuarioId) return listarLocal();

        try {
            const data = await ZeroFrameApi.request(`/api/usuarios/${usuarioId}/favoritos`);
            return ZeroFrameApi.normalizarLista(data);
        } catch {
            return listarLocal();
        }
    }

    async function alternar(produto) {
        const usuarioId = ZeroFrameApi.getUsuarioId();
        const produtoId = ProdutoService.getProdutoId(produto);

        if (usuarioId) {
            try {
                return await ZeroFrameApi.request(`/api/usuarios/${usuarioId}/favoritos/${produtoId}`, {
                    method: "POST"
                });
            } catch {
                // A API atual não expõe favoritos no Swagger; mantemos fallback local.
            }
        }

        const favoritos = listarLocal();
        const existe = favoritos.some((item) => ProdutoService.getProdutoId(item) === produtoId);
        salvarLocal(existe ? favoritos.filter((item) => ProdutoService.getProdutoId(item) !== produtoId) : [...favoritos, produto]);
        return true;
    }

    return { listar, alternar };
})();
