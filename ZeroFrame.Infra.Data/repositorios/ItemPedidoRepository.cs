using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.BDconexao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.repositorios
{
    public class ItemPedidoRepository : IItemPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemPedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemPedido?> ObterPorIdAsync(int id)
        {
            return await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<ItemPedido>> ObterPorPedidoAsync(int pedidoId)
        {
            return await _context.itemPedidos
                .Include(i => i.VariacaoProduto)
                .Where(i => i.PedidoId == pedidoId)
                .ToListAsync();
        }

        public async Task<List<ItemPedido>> ObterTodosAsync()
        {
            return await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                .ToListAsync();
        }

        public async Task AdicionarAsync(ItemPedido itemPedido)
        {
            await _context.itemPedidos.AddAsync(itemPedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(ItemPedido itemPedido)
        {
            _context.itemPedidos.Update(itemPedido);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var itemPedido = await _context.itemPedidos.FindAsync(id);

            if (itemPedido is null)
                return;

            _context.itemPedidos.Remove(itemPedido);
            await _context.SaveChangesAsync();
        }
    }
}

