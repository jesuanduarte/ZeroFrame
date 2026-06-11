using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Pedidos;

namespace ZeroFrame.Application.Interfaces
{
    // 
    public interface IPedidoService
    {
        Task<List<PedidosGetDto>> ObterTodosAsync();
        Task<PagedResponse<PedidosGetDto>> ObterTodosPaginadoAsync(PaginationParams paginationParams);
        Task<PedidosGetDto?> ObterPorIdAsync(int id);
        Task<List<PedidosGetDto>> ObterPorUsuarioAsync(int usuario);
        Task<PagedResponse<PedidosGetDto>> ObterPorUsuarioPaginadoAsync(int usuarioId, PaginationParams paginationParams);
        Task<PedidosGetDto> CriarAsync(PedidosPostDto pedidosPostDto);
        Task<PedidosGetDto> CriarAPartirDoCarrinhoAsync(int carrinhoId, int enderecoId);
        Task<PedidosGetDto> CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(int usuarioId, int enderecoId);
        Task AtualizarStatusAsync(int pedidoId, PedidoStatusUpdateDto pedidoStatusUpdateDto, bool usuarioAdministrador);
        Task AtualizarStatusEntregaAsync(int pedidoId, PedidoStatusEntregaUpdateDto pedidoStatusEntregaUpdateDto, bool usuarioAdministrador);
        Task CancelarAsync(int id);
    }
}
