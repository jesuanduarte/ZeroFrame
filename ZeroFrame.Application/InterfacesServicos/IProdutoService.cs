using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Produto;

namespace ZeroFrame.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<List<ProdutoGetDto>> ObterTodosAsync();
        Task<List<ProdutoGetDto>> ObterTodosAsync(ProdutoFiltroDto filtro);
        Task<PagedResponse<ProdutoGetDto>> ObterTodosPaginadoAsync(ProdutoFiltroDto filtro, PaginationParams paginationParams);
        Task<List<ProdutoGetDto>> ObterTodosAdminAsync();
        Task<PagedResponse<ProdutoGetDto>> ObterTodosAdminPaginadoAsync(PaginationParams paginationParams);
        Task<ProdutoGetDto?> ObterPorIdAsync(int id);
        Task<ProdutoGetDto> CriarAsync(ProdutoPostDto produtoPostDto);
        Task AtualizarAsync(ProdutoPutDto produtoPutDto);
        Task RemoverAsync(int id);
    }
}
