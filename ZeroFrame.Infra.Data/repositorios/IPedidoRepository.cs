using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.BDconexao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.repositorios
{
    // Classe que implementa o repositório da entidade.
    // Ela contém os métodos responsáveis por manipular os dados no sistema.
    public class PedidoRepository : IPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pedidos?> ObterPorIdAsync(int id)
        {
            return await _context.pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .Include(p => p.Pagamento)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Pedidos>> ObterPorUsuarioAsync(int usuarioId)
        {
            return await _context.pedidos
                .Include(p => p.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .Include(p => p.Pagamento)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task CriarPedidoAsync(Pedidos pedido)
        {
            await _context.pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarPedidoAsync(Pedidos pedido)
        {
            _context.pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pedidos pedido)
        {
            await AtualizarPedidoAsync(pedido);
        }

        public async Task CancelarPedidoAsync(int id)
        {
            var pedido = await _context.pedidos.FindAsync(id);

            if (pedido is null)
                return;

            pedido.Status = "Cancelado";

            _context.pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
    }
}
