using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.Repositorios
{
    // Classe que implementa o repositório da entidade.
    // Ela contém os métodos responsáveis por manipular os dados no sistema.
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly ApplicationDbContext _context;

        public PagamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Busca um pagamento pelo Id
        public async Task<Pagamento?> ObterPorIdAsync(int id)
        {
            return await _context.Pagamentos
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Busca um pagamento pelo Id do pedido
        public async Task<Pagamento?> ObterPorPedidoIdAsync(int pedidoId)
        {
            return await _context.Pagamentos
                .Include(p => p.Pedido)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
        }

        // Busca todos os pagamentos
        public async Task AdicionarAsync(Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
        }

        // Atualiza um pagamento existente
        public async Task AtualizarAsync(Pagamento pagamento)
        {
            _context.Pagamentos.Update(pagamento);
            await _context.SaveChangesAsync();
        }
    }
}
