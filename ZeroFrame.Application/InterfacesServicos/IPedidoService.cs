using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Pedidos;

namespace ZeroFrame.Application.Interfaces
{
    // 
    public interface IPedidoService
    {
        Task<PedidosGetDto?> ObterPorIdAsync(int id);
        Task<List<PedidosGetDto>> ObterPorUsuarioAsync(int usuario);
        Task<PedidosGetDto> CriarAsync(PedidosPostDto pedidosPostDto);
        Task<PedidosGetDto> CriarAPartirDoCarrinhoAsync(int carrinhoId);
        Task<PedidosGetDto> CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(int usuarioId);
        Task AtualizarAsync(PedidosPutDto pedidosPutDto);
        Task CancelarAsync(int id);
    }
}
