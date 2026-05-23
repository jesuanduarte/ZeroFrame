using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Categoria;

namespace ZeroFrame.Application.Interfaces
{ 
    public interface ICategoriaService
    {
        Task<List<CategoriaGetDto>> ObterTodosAsync();
        Task<PagedResponse<CategoriaGetDto>> ObterTodosPaginadoAsync(PaginationParams paginationParams);
        Task<CategoriaGetDto?> ObterPorIdAsync(int id);
        Task<CategoriaGetDto> CriarAsync(CategoriaPostDto categoriaPostDto);
        Task AtualizarAsync(CategoriaPutDto categoriaPutDto);
        Task RemoverAsync(int id);
    }
}
