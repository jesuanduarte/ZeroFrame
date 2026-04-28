using ZeroFrame.domain.entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.domain.Interface
{
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