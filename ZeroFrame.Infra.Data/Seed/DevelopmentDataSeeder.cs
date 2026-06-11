using Microsoft.EntityFrameworkCore;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Infra.Data.Context;

namespace ZeroFrame.Infra.Data.Seed
{
    public static class DevelopmentDataSeeder
    {
        private const string AdminEmail = "zeroframe@gmail.com";
        private const string SeedImagePublicPath = "/uploads/produtos/seed/";

        public static async Task SeedAsync(ApplicationDbContext context, string webRootPath)
        {
            // Seed usada apenas em Development para dados iniciais de teste.
            // Os metodos abaixo evitam duplicacao ao verificar email, categoria, produto e variacao.
            EnsureSeedImages(webRootPath);
            await SeedAdministradorAsync(context);
            await SeedCatalogoAsync(context);
        }

        private static async Task SeedAdministradorAsync(ApplicationDbContext context)
        {
            var emailNormalizado = AdminEmail.Trim().ToLower();
            var administradorExiste = await context.Usuarios
                .AnyAsync(usuario => usuario.Email.ToLower() == emailNormalizado);

            if (administradorExiste)
                return;

            var administrador = new Usuario
            {
                Nome = "Administrador ZeroFrame",
                Email = AdminEmail,
                Senha = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Telefone = "11999999999",
                Ativo = true,
                Perfil = "Administrador"
            };

            await context.Usuarios.AddAsync(administrador);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCatalogoAsync(ApplicationDbContext context)
        {
            var categorias = new[]
            {
                new CategoriaSeed("Moletons", "Moletons e blusas para o catalogo inicial de teste."),
                new CategoriaSeed("Camisetas", "Camisetas casuais para o catalogo inicial de teste."),
                new CategoriaSeed("Calças", "Calcas e pecas inferiores para o catalogo inicial de teste.")
            };

            foreach (var categoriaSeed in categorias)
                await ObterOuCriarCategoriaAsync(context, categoriaSeed);

            await context.SaveChangesAsync();

            var categoriasPorNome = await context.categorias
                .ToDictionaryAsync(categoria => categoria.Nome.ToLower(), categoria => categoria);

            var produtos = new[]
            {
                new ProdutoSeed(
                    Nome: "Moletom Preto No Money But Love",
                    Descricao: "Moletom preto com capuz, estampa traseira floral e frase No Money But Love.",
                    CategoriaNome: "Moletons",
                    Preco: 189.90m,
                    PrecoCusto: 120.00m,
                    ImagemUrl: SeedImagePublicPath + "blusa.webp",
                    Marca: "Zero Frame",
                    Origem: "Original",
                    Genero: "Unissex",
                    Cor: "Preto",
                    SecaoVitrine: "Recomendados",
                    TipoTamanho: "Letra",
                    TamanhosDisponiveis: "P,M,G,GG",
                    Variacoes:
                    [
                        new VariacaoSeed("P", "Preto", 10),
                        new VariacaoSeed("M", "Preto", 15),
                        new VariacaoSeed("G", "Preto", 12),
                        new VariacaoSeed("GG", "Preto", 8)
                    ]),
                new ProdutoSeed(
                    Nome: "Camiseta Branca Estampa Câmera",
                    Descricao: "Camiseta branca casual com pequena estampa frontal de câmera fotográfica.",
                    CategoriaNome: "Camisetas",
                    Preco: 79.90m,
                    PrecoCusto: 42.00m,
                    ImagemUrl: SeedImagePublicPath + "camisa-branca.webp",
                    Marca: "Zero Frame",
                    Origem: "Original",
                    Genero: "Unissex",
                    Cor: "Branco",
                    SecaoVitrine: "Recomendados",
                    TipoTamanho: "Letra",
                    TamanhosDisponiveis: "P,M,G,GG",
                    Variacoes:
                    [
                        new VariacaoSeed("P", "Branco", 15),
                        new VariacaoSeed("M", "Branco", 20),
                        new VariacaoSeed("G", "Branco", 15),
                        new VariacaoSeed("GG", "Branco", 10)
                    ]),
                new ProdutoSeed(
                    Nome: "Calça Jeans Preta Wide Leg",
                    Descricao: "Calça jeans preta de cintura alta, modelagem wide leg, com bolsos funcionais e costuras reforçadas.",
                    CategoriaNome: "Calças",
                    Preco: 159.90m,
                    PrecoCusto: 95.00m,
                    ImagemUrl: SeedImagePublicPath + "calca-jeans-preta-wide-leg.webp",
                    Marca: "Multimarcas",
                    Origem: "Multimarcas",
                    Genero: "Feminino",
                    Cor: "Preto",
                    SecaoVitrine: "Recomendados",
                    TipoTamanho: "Numero",
                    TamanhosDisponiveis: "36,38,40,42",
                    Variacoes:
                    [
                        new VariacaoSeed("36", "Preto", 8),
                        new VariacaoSeed("38", "Preto", 12),
                        new VariacaoSeed("40", "Preto", 10),
                        new VariacaoSeed("42", "Preto", 6)
                    ]),
                new ProdutoSeed(
                    Nome: "Calça Preta Streetwear",
                    Descricao: "Calça preta estilo streetwear, modelagem larga, confortável e casual.",
                    CategoriaNome: "Calças",
                    Preco: 139.90m,
                    PrecoCusto: 82.00m,
                    ImagemUrl: SeedImagePublicPath + "calca-preta-streetwear.webp",
                    Marca: "Multimarcas",
                    Origem: "Multimarcas",
                    Genero: "Unissex",
                    Cor: "Preto",
                    SecaoVitrine: "Recomendados",
                    TipoTamanho: "Letra",
                    TamanhosDisponiveis: "P,M,G,GG",
                    Variacoes:
                    [
                        new VariacaoSeed("P", "Preto", 10),
                        new VariacaoSeed("M", "Preto", 15),
                        new VariacaoSeed("G", "Preto", 10),
                        new VariacaoSeed("GG", "Preto", 5)
                    ])
            };

            foreach (var produtoSeed in produtos)
                await ObterOuCriarProdutoAsync(context, produtoSeed, categoriasPorNome);

            await context.SaveChangesAsync();
        }

        private static async Task ObterOuCriarCategoriaAsync(ApplicationDbContext context, CategoriaSeed categoriaSeed)
        {
            var nomeNormalizado = categoriaSeed.Nome.ToLower();
            var categoriaExiste = await context.categorias
                .AnyAsync(categoria => categoria.Nome.ToLower() == nomeNormalizado);

            if (categoriaExiste)
                return;

            context.categorias.Add(new Categoria
            {
                Nome = categoriaSeed.Nome,
                Descricao = categoriaSeed.Descricao
            });
        }

        private static async Task ObterOuCriarProdutoAsync(
            ApplicationDbContext context,
            ProdutoSeed produtoSeed,
            Dictionary<string, Categoria> categoriasPorNome)
        {
            var nomeNormalizado = produtoSeed.Nome.ToLower();
            var produto = await context.produtos
                .Include(p => p.VariacoesProdutos)
                .FirstOrDefaultAsync(p => p.Nome.ToLower() == nomeNormalizado);

            if (produto == null)
            {
                var categoria = categoriasPorNome[produtoSeed.CategoriaNome.ToLower()];

                produto = new Produto
                {
                    Nome = produtoSeed.Nome,
                    Descricao = produtoSeed.Descricao,
                    Preco = produtoSeed.Preco,
                    PrecoCusto = produtoSeed.PrecoCusto,
                    PrecoOriginal = null,
                    TipoDesconto = "nenhum",
                    Desconto = 0m,
                    Genero = produtoSeed.Genero,
                    Cor = produtoSeed.Cor,
                    SecaoVitrine = produtoSeed.SecaoVitrine,
                    TipoTamanho = produtoSeed.TipoTamanho,
                    TamanhosDisponiveis = produtoSeed.TamanhosDisponiveis,
                    // O banco salva somente o caminho publico da imagem principal.
                    // A estrutura atual ainda nao possui tabela para imagens extras do produto.
                    ImagemUrl = produtoSeed.ImagemUrl,
                    Marca = produtoSeed.Marca,
                    Origem = produtoSeed.Origem,
                    CategoriaId = categoria.Id,
                    Ativo = true
                };

                context.produtos.Add(produto);
            }

            foreach (var variacaoSeed in produtoSeed.Variacoes)
                AdicionarVariacaoSeNaoExistir(produto, variacaoSeed);
        }

        private static void AdicionarVariacaoSeNaoExistir(Produto produto, VariacaoSeed variacaoSeed)
        {
            var variacaoExiste = produto.VariacoesProdutos.Any(variacao =>
                variacao.Tamanho.Equals(variacaoSeed.Tamanho, StringComparison.OrdinalIgnoreCase)
                && variacao.Cor.Equals(variacaoSeed.Cor, StringComparison.OrdinalIgnoreCase));

            if (variacaoExiste)
                return;

            produto.VariacoesProdutos.Add(new VariacaoProdutos
            {
                Tamanho = variacaoSeed.Tamanho,
                Cor = variacaoSeed.Cor,
                Estoque = variacaoSeed.Estoque
            });
        }

        private static void EnsureSeedImages(string webRootPath)
        {
            var uploadsPath = Path.Combine(webRootPath, "uploads", "produtos");
            var seedPath = Path.Combine(uploadsPath, "seed");
            Directory.CreateDirectory(seedPath);

            var images = new[]
            {
                new SeedImage("efd6899af1d74dd78605631caa56a16b.webp", "blusa.webp"),
                new SeedImage("ea2ff0a49c874920a2c89fafcba889ec.webp", "blusa-preta.webp"),
                new SeedImage("6f047eb9601e4868b4bc7ecb3a901ac2.webp", "camisa-branca.webp"),
                new SeedImage("6f047eb9601e4868b4bc7ecb3a901ac2.webp", "camisa-branca-2.webp"),
                new SeedImage("6109f63d87264cd6afc88e7a54071f39.webp", "calca-jeans-preta-wide-leg.webp"),
                new SeedImage("6109f63d87264cd6afc88e7a54071f39.webp", "calca-preta-streetwear.webp")
            };

            foreach (var image in images)
            {
                var destination = Path.Combine(seedPath, image.DestinationFileName);

                if (File.Exists(destination))
                    continue;

                var source = Path.Combine(uploadsPath, image.SourceFileName);
                if (File.Exists(source))
                    File.Copy(source, destination);
            }
        }

        private sealed record CategoriaSeed(string Nome, string Descricao);

        private sealed record VariacaoSeed(string Tamanho, string Cor, int Estoque);

        private sealed record SeedImage(string SourceFileName, string DestinationFileName);

        private sealed record ProdutoSeed(
            string Nome,
            string Descricao,
            string CategoriaNome,
            decimal Preco,
            decimal PrecoCusto,
            string ImagemUrl,
            string Marca,
            string Origem,
            string Genero,
            string Cor,
            string SecaoVitrine,
            string TipoTamanho,
            string TamanhosDisponiveis,
            IReadOnlyCollection<VariacaoSeed> Variacoes);
    }
}
