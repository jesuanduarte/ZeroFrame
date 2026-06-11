using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.ItemCarrinho;

namespace ZeroFrame.Application.Interfaces
{
    public interface IItemCarrinhoService
    {
        Task<List<ItemCarrinhoGetDto>> ObterTodosAsync();
        Task<ItemCarrinhoGetDto?> ObterPorIdAsync(int id);
        Task<List<ItemCarrinhoGetDto>> ObterPorCarrinhoAsync(int carrinho);
        Task<ItemCarrinhoGetDto> CriarAsync(ItemCarrinhoPostDto itemCarrinhoPostDto);
        Task AtualizarAsync(ItemCarrinhoPutDto itemCarrinhoPutDto);
        Task RemoverAsync(int id);
    }
}