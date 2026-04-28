using ZeroFrame.domain.entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ZeroFrame.domain.Interface
{
    public interface IItemPedidoRepository
    {
        Task<ItemPedido?> ObterPorIdAsync(int id);

        Task<List<ItemPedido>> ObterPorPedidoAsync(int pedidoId);

        Task AdicionarAsync(ItemPedido itemPedido);
        Task AtualizarAsync(ItemPedido itemPedido);
        Task RemoverAsync(int id);
        Task<List<ItemPedido>> ObterTodosAsync();
    }
}

