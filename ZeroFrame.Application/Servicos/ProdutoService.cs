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

        public async Task<List<ProdutoGetDto>> ObterTodosAsync()
        {
            return await ObterTodosAsync(new ProdutoFiltroDto());
        }

        public async Task<List<ProdutoGetDto>> ObterTodosAsync(ProdutoFiltroDto filtro)
        {
            var produtos = await _produtoRepository.ObterTodosAsync();

            return produtos
                .Where(produto => AtendeFiltro(produto, filtro))
                .Select(MapearProdutoGetDto)
                .ToList();
        }

        public async Task<ProdutoGetDto?> ObterPorIdAsync(int id)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);

            if (produto == null)
                return null;

            return MapearProdutoGetDto(produto);
        }

        public async Task<ProdutoGetDto> CriarAsync(ProdutoPostDto produtoPostDto)
        {
            await ValidarCategoriaAsync(produtoPostDto.CategoriaId);

            var produto = new Produto
            {
                Nome = produtoPostDto.Nome,
                Descricao = produtoPostDto.Descricao,
                Preco = produtoPostDto.Preco,
                CategoriaId = produtoPostDto.CategoriaId,
                Ativo = true
            };

            await _produtoRepository.AdicionarAsync(produto);

            return MapearProdutoGetDto(produto);
        }

        public async Task AtualizarAsync(ProdutoPutDto produtoPutDto)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(produtoPutDto.Id);

            if (produto == null)
                return;

            await ValidarCategoriaAsync(produtoPutDto.CategoriaId);

            produto.Nome = produtoPutDto.Nome;
            produto.Descricao = produtoPutDto.Descricao;
            produto.Preco = produtoPutDto.Preco;
            produto.CategoriaId = produtoPutDto.CategoriaId;
            produto.Ativo = produtoPutDto.Ativo;

            await _produtoRepository.AtualizarAsync(produto);
        }

        public async Task RemoverAsync(int id)
        {
            await _produtoRepository.RemoverAsync(id);
        }

        private async Task ValidarCategoriaAsync(int categoriaId)
        {
            var categoria = await _categoriaRepository.ObterPorIdAsync(categoriaId);

            if (categoria == null)
                throw new InvalidOperationException("Categoria nao encontrada.");
        }

        private static bool AtendeFiltro(Produto produto, ProdutoFiltroDto filtro)
        {
            return AtendeBusca(produto, filtro.Busca)
                && AtendeCategoria(produto, filtro.Categoria)
                && AtendeMarca(produto, filtro.Marca)
                && AtendeOrigem(produto, filtro.Origem)
                && AtendePreco(produto, filtro.PrecoMin, filtro.PrecoMax)
                && AtendeVariacao(produto, filtro.Tamanho, filtro.Cor);
        }

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

        private static bool AtendeCategoria(Produto produto, string? categoria)
        {
            return string.IsNullOrWhiteSpace(categoria)
                || Contem(produto.Categoria?.Nome, categoria)
                || produto.CategoriaId.ToString() == categoria.Trim();
        }

        private static bool AtendeMarca(Produto produto, string? marca)
        {
            return string.IsNullOrWhiteSpace(marca) || Contem(ObterMarca(produto), marca);
        }

        private static bool AtendeOrigem(Produto produto, string? origem)
        {
            return string.IsNullOrWhiteSpace(origem) || Contem(ObterOrigem(produto), origem);
        }

        private static bool AtendePreco(Produto produto, decimal? precoMin, decimal? precoMax)
        {
            if (precoMin.HasValue && produto.Preco < precoMin.Value)
                return false;

            if (precoMax.HasValue && produto.Preco > precoMax.Value)
                return false;

            return true;
        }

        private static bool AtendeVariacao(Produto produto, string? tamanho, string? cor)
        {
            if (string.IsNullOrWhiteSpace(tamanho) && string.IsNullOrWhiteSpace(cor))
                return true;

            return produto.VariacoesProdutos.Any(variacao =>
                (string.IsNullOrWhiteSpace(tamanho) || Contem(variacao.Tamanho, tamanho))
                && (string.IsNullOrWhiteSpace(cor) || Contem(variacao.Cor, cor)));
        }

        private static bool Contem(string? valor, string filtro)
        {
            return !string.IsNullOrWhiteSpace(valor)
                && valor.Contains(filtro.Trim(), StringComparison.OrdinalIgnoreCase);
        }


        private static decimal? ObterPrecoOriginal(Produto produto)
        {
            return null;
        }

        private static string ObterImagemUrl(Produto produto)
        {
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

        private static string ObterMarca(Produto produto)
        {
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("nike") || nomeNormalizado.Contains("jordan"))
                return "Nike";

            if (nomeNormalizado.Contains("adidas"))
                return "Adidas";

            if (nomeNormalizado.Contains("levis") || nomeNormalizado.Contains("levi"))
                return "Levi's";

            return "Zero Frame";
        }

        private static string ObterOrigem(Produto produto)
        {
            return ObterMarca(produto) == "Zero Frame" ? "Original" : "Multimarcas";
        }

        private static ProdutoGetDto MapearProdutoGetDto(Produto produto)
        {
            return new ProdutoGetDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                PrecoOriginal = ObterPrecoOriginal(produto),
                EmPromocao = ObterPrecoOriginal(produto).HasValue && ObterPrecoOriginal(produto) > produto.Preco,
                ImagemUrl = ObterImagemUrl(produto),
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome ?? string.Empty,
                Marca = ObterMarca(produto),
                Origem = ObterOrigem(produto),
                Ativo = produto.Ativo,
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