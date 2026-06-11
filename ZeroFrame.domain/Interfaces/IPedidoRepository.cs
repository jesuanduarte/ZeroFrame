using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositório de pedidos.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IPedidoRepository
    {
        Task<List<Pedidos>> ObterTodosAsync();
        Task<(List<Pedidos> Items, int TotalItems)> ObterTodosPaginadoAsync(int pageNumber, int pageSize);
        Task<Pedidos?> ObterPorIdAsync(int id);
        Task<List<Pedidos>> ObterPorUsuarioAsync(int usuarioId);
        Task<(List<Pedidos> Items, int TotalItems)> ObterPorUsuarioPaginadoAsync(int usuarioId, int pageNumber, int pageSize);
        Task CriarPedidoAsync(Pedidos pedido);
        Task AtualizarPedidoAsync(Pedidos pedido);
        Task CancelarPedidoAsync(int id);
        Task AtualizarAsync(Pedidos pedido);
    }
}
