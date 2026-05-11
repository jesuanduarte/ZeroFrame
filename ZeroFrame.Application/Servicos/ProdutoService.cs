using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.Produto;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Filtros;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;

        private readonly ICategoriaRepository _categoriaRepository;

        public ProdutoService(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository)
        {
            _produtoRepository = produtoRepository;
            _categoriaRepository = categoriaRepository;
        }

        // Busca todos os produtos sem aplicar filtro.
        public async Task<List<ProdutoGetDto>> ObterTodosAsync()
        {
            return await ObterTodosAsync(new ProdutoFiltroDto());
        }

        // Busca todos os produtos e aplica os filtros informados.
        public async Task<List<ProdutoGetDto>> ObterTodosAsync(ProdutoFiltroDto filtro)
        {
            var produtos = await _produtoRepository.ObterTodosAsync(MapearProdutoFiltro(filtro));

            return produtos
                .Select(MapearProdutoGetDto)
                .ToList();
        }

        // Busca um produto pelo Id.
        public async Task<ProdutoGetDto?> ObterPorIdAsync(int id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto == null)
                return null;

            return MapearProdutoGetDto(produto);
        }

        // Cria um novo produto.
        public async Task<ProdutoGetDto> CriarAsync(ProdutoPostDto produtoPostDto)
        {
            // Valida se a categoria informada existe antes de criar o produto.
            await ValidarCategoriaAsync(produtoPostDto.CategoriaId);

            // Monta a entidade Produto com os dados recebidos do DTO.
            var produto = new Produto
            {
                Nome = produtoPostDto.Nome,
                Descricao = produtoPostDto.Descricao,
                Preco = produtoPostDto.Preco,
                PrecoOriginal = produtoPostDto.PrecoOriginal,
                ImagemUrl = produtoPostDto.ImagemUrl ?? string.Empty,
                Marca = produtoPostDto.Marca ?? string.Empty,
                Origem = produtoPostDto.Origem ?? string.Empty,
                CategoriaId = produtoPostDto.CategoriaId,
                Ativo = true
            };

            // Salva o produto no banco.
            await _produtoRepository.AdicionarAsync(produto);

            // Retorna o produto criado convertido para DTO.
            return MapearProdutoGetDto(produto);
        }

        // Atualiza os dados de um produto existente.
        public async Task AtualizarAsync(ProdutoPutDto produtoPutDto)
        {
            // Busca o produto pelo Id.
            var produto = await _produtoRepository.ObterPorIdAsync(produtoPutDto.Id);

            // Se não encontrar o produto, encerra o método.
            if (produto == null)
                return;

            // Valida se a nova categoria informada existe.
            await ValidarCategoriaAsync(produtoPutDto.CategoriaId);

            // Atualiza os dados da entidade com os dados recebidos do DTO.
            produto.Nome = produtoPutDto.Nome;
            produto.Descricao = produtoPutDto.Descricao;
            produto.Preco = produtoPutDto.Preco;
            produto.PrecoOriginal = produtoPutDto.PrecoOriginal;
            produto.ImagemUrl = produtoPutDto.ImagemUrl ?? string.Empty;
            produto.Marca = produtoPutDto.Marca ?? string.Empty;
            produto.Origem = produtoPutDto.Origem ?? string.Empty;
            produto.CategoriaId = produtoPutDto.CategoriaId;
            produto.Ativo = produtoPutDto.Ativo;

            // Salva a atualização no banco.
            await _produtoRepository.AtualizarAsync(produto);
        }

        // Remove um produto pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _produtoRepository.RemoverAsync(id);
        }

        // Verifica se a categoria existe antes de cadastrar ou atualizar produto.
        private async Task ValidarCategoriaAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(categoriaId);

            if (categoria == null)
                throw new InvalidOperationException("Categoria nao encontrada.");
        }

        private static ProdutoFiltro MapearProdutoFiltro(ProdutoFiltroDto filtro)
        {
            return new ProdutoFiltro
            {
                Busca = filtro.Busca,
                Categoria = filtro.Categoria,
                Marca = filtro.Marca,
                Origem = filtro.Origem,
                Tamanho = filtro.Tamanho,
                Cor = filtro.Cor,
                PrecoMin = filtro.PrecoMin,
                PrecoMax = filtro.PrecoMax
            };
        }

        // Retorna o preço original do produto, se existir.
        private static decimal? ObterPrecoOriginal(Produto produto)
        {
            return produto.PrecoOriginal;
        }

        // Retorna a imagem do produto.
        // Se o produto não tiver imagem cadastrada, escolhe uma imagem padrão com base no nome.
        private static string ObterImagemUrl(Produto produto)
        {
            if (!string.IsNullOrWhiteSpace(produto.ImagemUrl))
                return produto.ImagemUrl;

            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("jordan") || nomeNormalizado.Contains("latte"))
                return "/assets/products/aj1-high-latte.png";

            if (nomeNormalizado.Contains("camisa") || nomeNormalizado.Contains("oversized"))
                return "/assets/products/camisa-over-black.png";

            if (nomeNormalizado.Contains("bermuda"))
                return "/assets/products/bermuda-jeans.jpg";

            if (nomeNormalizado.Contains("moletom") || nomeNormalizado.Contains("blusa"))
                return "/assets/products/blusa-moletom.jpg";

            if (nomeNormalizado.Contains("calca") || nomeNormalizado.Contains("calça") || nomeNormalizado.Contains("jeans"))
                return "/assets/products/calca-levis-clara.png";

            if (nomeNormalizado.Contains("corrente") || nomeNormalizado.Contains("ice"))
                return "/assets/products/corrente-ice.png";

            if (nomeNormalizado.Contains("adidas") || nomeNormalizado.Contains("tenis") || nomeNormalizado.Contains("tênis"))
                return "/assets/products/tenis2.png";

            return "/assets/products/camisa-over-black.png";
        }

        // Retorna a marca do produto.
        // Se a marca não estiver cadastrada, tenta descobrir pelo nome do produto.
        private static string ObterMarca(Produto produto)
        {
            if (!string.IsNullOrWhiteSpace(produto.Marca))
                return produto.Marca;

            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("nike") || nomeNormalizado.Contains("jordan"))
                return "Nike";

            if (nomeNormalizado.Contains("adidas"))
                return "Adidas";

            if (nomeNormalizado.Contains("levis") || nomeNormalizado.Contains("levi"))
                return "Levi's";

            return "Zero Frame";
        }

        // Retorna a origem do produto.
        // Se não tiver origem cadastrada, define como Original ou Multimarcas.
        private static string ObterOrigem(Produto produto)
        {
            if (!string.IsNullOrWhiteSpace(produto.Origem))
                return produto.Origem;

            return ObterMarca(produto) == "Zero Frame" ? "Original" : "Multimarcas";
        }

        // Converte a entidade Produto para ProdutoGetDto.
        private static ProdutoGetDto MapearProdutoGetDto(Produto produto)
        {
            return new ProdutoGetDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                PrecoOriginal = ObterPrecoOriginal(produto),

                // Define se o produto está em promoção.
                EmPromocao = ObterPrecoOriginal(produto).HasValue && ObterPrecoOriginal(produto) > produto.Preco,

                ImagemUrl = ObterImagemUrl(produto),
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome ?? string.Empty,
                Marca = ObterMarca(produto),
                Origem = ObterOrigem(produto),
                Ativo = produto.Ativo,

                // Converte as variações do produto para DTO.
                Variacoes = produto.VariacoesProdutos.Select(variacao => new VariacaoGetDto
                {
                    Id = variacao.Id,
                    Tamanho = variacao.Tamanho,
                    Cor = variacao.Cor,
                    Estoque = variacao.Estoque,
                    ProdutoId = variacao.ProdutoId
                }).ToList()
            };
        }
    }
}
