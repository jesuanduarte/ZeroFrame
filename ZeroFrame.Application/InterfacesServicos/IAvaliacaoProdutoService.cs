using ZeroFrame.Application.DTOS.AvaliacaoProduto;
using ZeroFrame.Application.DTOS.Common;

namespace ZeroFrame.Application.Interfaces
{
    public interface IAvaliacaoProdutoService
    {
        Task<AvaliacaoProdutoGetDto> CriarAsync(int produtoId, AvaliacaoProdutoPostDto dto, int? usuarioId);
        Task<AvaliacaoProdutoGetDto> AtualizarMinhaAvaliacaoAsync(int produtoId, AvaliacaoProdutoPutDto dto, int? usuarioId);
        Task<List<AvaliacaoProdutoGetDto>> ListarAtivasPorProdutoAsync(int produtoId);
        Task<PagedResponse<AvaliacaoProdutoGetDto>> ListarAtivasPorProdutoPaginadoAsync(int produtoId, PaginationParams paginationParams);
        Task<AvaliacaoResumoDto> ObterResumoAsync(int produtoId);
        Task DesativarAsync(int avaliacaoId, bool usuarioAdministrador);
        Task ApagarAsync(int avaliacaoId, bool usuarioAdministrador);
        Task<AvaliacaoProdutoGetDto> ModerarComentarioAsync(int avaliacaoId, AvaliacaoProdutoComentarioModeracaoDto dto, bool usuarioAdministrador);
    }
}
