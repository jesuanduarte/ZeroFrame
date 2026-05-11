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
        Task<Pedidos?> ObterPorIdAsync(int id);
        Task<List<Pedidos>> ObterPorUsuarioAsync(int usuarioId);
        Task CriarPedidoAsync(Pedidos pedido);
        Task AtualizarPedidoAsync(Pedidos pedido);
        Task CancelarPedidoAsync(int id);
        Task AtualizarAsync(Pedidos pedido);
    }
}