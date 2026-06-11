using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.ItemPedido;

namespace ZeroFrame.Application.Interfaces
{
    public interface IItemPedidoService
    {
        Task<ItemPedidoGetDto?> ObterPorIdAsync(int id);
        Task<List<ItemPedidoGetDto>> ObterPorPedidoAsync(int pedido);
        Task<ItemPedidoGetDto> CriarAsync(ItemPedidoPostDto itemPedidoPostDto);
        Task AtualizarAsync(ItemPedidoPutDto itemPedidoPutDto);
        Task RemoverAsync(int id);
        Task<List<ItemPedidoGetDto>> ObterTodosAsync();
    }
}
