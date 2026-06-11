using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces

{
    // Interface que define o contrato do repositório de pagamentos.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IPagamentoRepository
    {
        Task<Pagamento?> ObterPorIdAsync(int id);
        Task<Pagamento?> ObterPorPedidoIdAsync(int pedidoId);
        Task AdicionarAsync(Pagamento pagamento);
        Task AtualizarAsync(Pagamento pagamento);
    }
}
