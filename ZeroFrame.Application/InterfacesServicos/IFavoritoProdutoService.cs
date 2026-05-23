using ZeroFrame.Application.DTOS.FavoritoProduto;

namespace ZeroFrame.Application.Interfaces
{
    public interface IFavoritoProdutoService
    {
        Task<List<FavoritoProdutoGetDto>> ObterPorUsuarioAsync(int usuarioId);
        Task<FavoritoProdutoGetDto> AdicionarAsync(int usuarioId, int produtoId);
        Task RemoverAsync(int usuarioId, int produtoId);
    }
}
