document.addEventListener("DOMContentLoaded", async () => {
    const containers = document.querySelectorAll(".products-container");
    if (!containers.length) return;

    try {
        containers.forEach((container) => ZeroFrameApi.mostrarCarregando(container, "Carregando produtos..."));
        const produtos = await ProdutoService.listar();
        if (!produtos.length) return;

        const recomendados = produtos.slice(0, 7);
        const originais = produtos.filter((produto) => String(produto.origem || produto.Origem || "").toLowerCase().includes("original")).slice(0, 7);
        const multimarcas = produtos.filter((produto) => String(produto.origem || produto.Origem || produto.marca || produto.Marca || "").toLowerCase().includes("multi")).slice(0, 7);
        const grupos = [recomendados, originais.length ? originais : recomendados, multimarcas.length ? multimarcas : recomendados];

        containers.forEach((container, index) => {
            if (index > 2) return;
            container.textContent = "";
            grupos[index].forEach((produto) => {
                container.appendChild(ProdutoService.criarCard(produto, {
                    productPath: "pages/produtos/product.html",
                    fallbackImage: "./assets/products/camisa-over-black.png"
                }));
            });
        });
    } catch (error) {
        containers.forEach((container) => ZeroFrameApi.mostrarMensagem(container, ZeroFrameApi.tratarErro(error, "Erro ao carregar produtos.")));
    }
});
