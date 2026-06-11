using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Filtros;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositorio de produtos.
    // Ela estabelece quais operacoes deverao existir na implementacao.
    public interface IProdutoRepository
    {
        Task<List<Produto>> ObterTodosAsync();
        Task<List<Produto>> ObterTodosAsync(ProdutoFiltro filtro);
        Task<(List<Produto> Items, int TotalItems)> ObterTodosPaginadoAsync(ProdutoFiltro filtro, int pageNumber, int pageSize);
        Task<List<Produto>> ObterTodosAdminAsync();
        Task<(List<Produto> Items, int TotalItems)> ObterTodosAdminPaginadoAsync(int pageNumber, int pageSize);
        Task<Produto?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task RemoverAsync(int id);
    }
}
