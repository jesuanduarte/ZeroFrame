using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Pagamento;

namespace ZeroFrame.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<PagamentoGetDto?> ObterPorIdAsync(int id);
        Task<PagamentoGetDto?> ObterPorPedidoIdAsync(int pedido);
        Task<PagamentoGetDto> CriarAsync(PagamentoPostDto pagamentoPostDto);
        Task AtualizarAsync(PagamentoPutDto pagamentoPutDto);
    }
}
