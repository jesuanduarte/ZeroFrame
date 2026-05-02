using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Produto;

namespace ZeroFrame.Application.Interfaces
{
    public interface IProdutoService
    {
        Task<List<ProdutoGetDto>> ObterTodosAsync();
        Task<List<ProdutoGetDto>> ObterTodosAsync(ProdutoFiltroDto filtro);
        Task<ProdutoGetDto?> ObterPorIdAsync(int id);
        Task<ProdutoGetDto> CriarAsync(ProdutoPostDto produtoPostDto);
        Task AtualizarAsync(ProdutoPutDto produtoPutDto);
        Task RemoverAsync(int id);
    }
}
