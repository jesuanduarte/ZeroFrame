using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Domain.Interfaces
{
    public interface IAvaliacaoProdutoRepository
    {
        Task<AvaliacaoProduto> CriarAsync(AvaliacaoProduto avaliacao);
        Task AtualizarAsync(AvaliacaoProduto avaliacao);
        Task<AvaliacaoProduto?> ObterPorIdAsync(int id);
        Task<AvaliacaoProduto?> ObterAtivaPorUsuarioEProdutoAsync(int usuarioId, int produtoId);
        Task<List<AvaliacaoProduto>> ListarAtivasPorProdutoAsync(int produtoId);
        Task<(List<AvaliacaoProduto> Items, int TotalItems)> ListarAtivasPorProdutoPaginadoAsync(int produtoId, int pageNumber, int pageSize);
        Task DesativarAsync(int id);
        Task ApagarAsync(int id);
        Task<bool> ExisteAsync(int id);
        Task<decimal> CalcularMediaAvaliacoesAsync(int produtoId);
        Task<List<AvaliacaoProduto>> BuscarResumoAvaliacoesAsync(int produtoId);
    }
}
