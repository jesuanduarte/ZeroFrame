using ZeroFrame.Application.DTOS.FavoritoProduto;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    public class FavoritoProdutoService : IFavoritoProdutoService
    {
        private readonly IFavoritoProdutoRepository _favoritoProdutoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProdutoRepository _produtoRepository;

        public FavoritoProdutoService(
            IFavoritoProdutoRepository favoritoProdutoRepository,
            IUsuarioRepository usuarioRepository,
            IProdutoRepository produtoRepository)
        {
            _favoritoProdutoRepository = favoritoProdutoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoRepository = produtoRepository;
        }

        public async Task<List<FavoritoProdutoGetDto>> ObterPorUsuarioAsync(int usuarioId)
        {
            await ValidarUsuarioAsync(usuarioId);

            var favoritos = await _favoritoProdutoRepository.ObterPorUsuarioAsync(usuarioId);
            return favoritos.Select(MapearFavoritoGetDto).ToList();
        }

        public async Task<FavoritoProdutoGetDto> AdicionarAsync(int usuarioId, int produtoId)
        {
            await ValidarUsuarioAsync(usuarioId);
            var produto = await ObterProdutoAtivoOuFalharAsync(produtoId);

            var favoritoExistente = await _favoritoProdutoRepository.ObterPorUsuarioEProdutoAsync(usuarioId, produtoId);
            if (favoritoExistente != null)
                return MapearFavoritoGetDto(favoritoExistente);

            var favorito = new FavoritoProduto
            {
                UsuarioId = usuarioId,
                ProdutoId = produtoId,
                Produto = produto,
                DataCriacao = DateTime.UtcNow
            };

            await _favoritoProdutoRepository.AdicionarAsync(favorito);
            return MapearFavoritoGetDto(favorito);
        }

        public async Task RemoverAsync(int usuarioId, int produtoId)
        {
            await ValidarUsuarioAsync(usuarioId);

            var favorito = await _favoritoProdutoRepository.ObterPorUsuarioEProdutoAsync(usuarioId, produtoId)
                ?? throw new KeyNotFoundException("Favorito nao encontrado.");

            await _favoritoProdutoRepository.RemoverAsync(favorito);
        }

        private async Task ValidarUsuarioAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);

            if (usuario == null || !usuario.Ativo)
                throw new KeyNotFoundException("Usuario nao encontrado.");
        }

        private async Task<Produto> ObterProdutoAtivoOuFalharAsync(int produtoId)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

            if (produto == null || !produto.Ativo)
                throw new KeyNotFoundException("Produto nao encontrado.");

            return produto;
        }

        private static FavoritoProdutoGetDto MapearFavoritoGetDto(FavoritoProduto favorito)
        {
            var produto = favorito.Produto;
            var precoFinal = produto == null ? 0m : ProdutoPrecoService.CalcularPrecoFinal(produto);

            return new FavoritoProdutoGetDto
            {
                Id = favorito.Id,
                UsuarioId = favorito.UsuarioId,
                ProdutoId = favorito.ProdutoId,
                NomeProduto = produto?.Nome ?? string.Empty,
                ImagemUrl = produto?.ImagemUrl ?? string.Empty,
                CategoriaNome = produto?.Categoria?.Nome ?? string.Empty,
                Marca = produto?.Marca ?? string.Empty,
                Origem = produto?.Origem ?? string.Empty,
                Preco = produto?.Preco ?? 0m,
                PrecoFinal = precoFinal,
                DataCriacao = favorito.DataCriacao,
                Produto = new ProdutoFavoritoGetDto
                {
                    Id = produto?.Id ?? favorito.ProdutoId,
                    Nome = produto?.Nome ?? string.Empty,
                    ImagemUrl = produto?.ImagemUrl ?? string.Empty,
                    CategoriaNome = produto?.Categoria?.Nome ?? string.Empty,
                    Marca = produto?.Marca ?? string.Empty,
                    Origem = produto?.Origem ?? string.Empty,
                    Preco = produto?.Preco ?? 0m,
                    PrecoFinal = precoFinal
                }
            };
        }
    }
}
