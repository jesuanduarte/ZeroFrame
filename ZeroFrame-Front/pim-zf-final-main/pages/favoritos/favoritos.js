document.addEventListener("DOMContentLoaded", async () => {
    if (!ZeroFrameApi.protegerPagina()) return;

    const lista = document.querySelector(".favorites-list");
    if (!lista) return;

    try {
        ZeroFrameApi.mostrarCarregando(lista, "Carregando favoritos...");
        const favoritos = await FavoritoService.listar();

        if (!favoritos.length) {
            ZeroFrameApi.mostrarMensagem(lista, "Você ainda não tem produtos favoritos.");
            return;
        }

        lista.textContent = "";
        favoritos.forEach((item) => {
            const produto = item.produto || item.Produto || item;
            lista.appendChild(ProdutoService.criarCard(produto, {
                productPath: "../../pages/produtos/product.html",
                fallbackImage: "../../assets/products/camisa-over-black.png",
                tagName: "article"
            }));
        });
    } catch (error) {
        ZeroFrameApi.mostrarMensagem(lista, ZeroFrameApi.tratarErro(error, "Erro ao carregar favoritos."));
    }
});
