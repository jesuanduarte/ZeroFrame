using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.Produto;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

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
            var produtos = await _produtoRepository.ObterTodosAsync();

            // Filtra os produtos e depois transforma cada entidade Produto em ProdutoGetDto.
            return produtos
                .Where(produto => AtendeFiltro(produto, filtro))
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

        // Verifica se o produto atende todos os filtros informados.
        private static bool AtendeFiltro(Produto produto, ProdutoFiltroDto filtro)
        {
            return AtendeBusca(produto, filtro.Busca)
                && AtendeCategoria(produto, filtro.Categoria)
                && AtendeMarca(produto, filtro.Marca)
                && AtendeOrigem(produto, filtro.Origem)
                && AtendePreco(produto, filtro.PrecoMin, filtro.PrecoMax)
                && AtendeVariacao(produto, filtro.Tamanho, filtro.Cor);
        }

        // Verifica se o texto pesquisado aparece no nome, descrição, categoria, marca ou origem.
        private static bool AtendeBusca(Produto produto, string? busca)
        {
            if (string.IsNullOrWhiteSpace(busca))
                return true;

            return Contem(produto.Nome, busca)
                || Contem(produto.Descricao, busca)
                || Contem(produto.Categoria?.Nome, busca)
                || Contem(ObterMarca(produto), busca)
                || Contem(ObterOrigem(produto), busca);
        }

        // Verifica se o produto pertence à categoria informada.
        private static bool AtendeCategoria(Produto produto, string? categoria)
        {
            return string.IsNullOrWhiteSpace(categoria)
                || Contem(produto.Categoria?.Nome, categoria)
                || produto.CategoriaId.ToString() == categoria.Trim();
        }

        // Verifica se o produto pertence à marca informada.
        private static bool AtendeMarca(Produto produto, string? marca)
        {
            return string.IsNullOrWhiteSpace(marca) || Contem(ObterMarca(produto), marca);
        }

        // Verifica se o produto pertence à origem informada.
        private static bool AtendeOrigem(Produto produto, string? origem)
        {
            return string.IsNullOrWhiteSpace(origem) || Contem(ObterOrigem(produto), origem);
        }

        // Verifica se o preço do produto está dentro do preço mínimo e máximo informados.
        private static bool AtendePreco(Produto produto, decimal? precoMin, decimal? precoMax)
        {
            if (precoMin.HasValue && produto.Preco < precoMin.Value)
                return false;

            if (precoMax.HasValue && produto.Preco > precoMax.Value)
                return false;

            return true;
        }

        // Verifica se o produto possui alguma variação com o tamanho ou cor informados.
        private static bool AtendeVariacao(Produto produto, string? tamanho, string? cor)
        {
            if (string.IsNullOrWhiteSpace(tamanho) && string.IsNullOrWhiteSpace(cor))
                return true;

            return produto.VariacoesProdutos.Any(variacao =>
                (string.IsNullOrWhiteSpace(tamanho) || Contem(variacao.Tamanho, tamanho))
                && (string.IsNullOrWhiteSpace(cor) || Contem(variacao.Cor, cor)));
        }

        // Verifica se um texto contém o filtro informado, ignorando letras maiúsculas e minúsculas.
        private static bool Contem(string? valor, string filtro)
        {
            return !string.IsNullOrWhiteSpace(valor)
                && valor.Contains(filtro.Trim(), StringComparison.OrdinalIgnoreCase);
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