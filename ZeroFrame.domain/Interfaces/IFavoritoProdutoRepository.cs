using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Domain.Interfaces
{
    public interface IFavoritoProdutoRepository
    {
        Task<List<FavoritoProduto>> ObterPorUsuarioAsync(int usuarioId);
        Task<FavoritoProduto?> ObterPorUsuarioEProdutoAsync(int usuarioId, int produtoId);
        Task AdicionarAsync(FavoritoProduto favorito);
        Task RemoverAsync(FavoritoProduto favorito);
    }
}
