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

        // Busca um item específico do pedido pelo Id do próprio ItemPedido.
        public async Task<ItemPedido?> ObterPorIdAsync(int id)
        {
            return await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // // Busca todos os itens que pertencem a um pedido específico.
        public async Task<List<ItemPedido>> ObterPorPedidoAsync(int pedidoId)
        {
            return await _context.itemPedidos
                .Include(i => i.VariacaoProduto)
                .Where(i => i.PedidoId == pedidoId)
                .ToListAsync();
        }

        // busca todos os itens do pedido
        public async Task<List<ItemPedido>> ObterTodosAsync()
        {
            return await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                .ToListAsync();
        }

        // adiciona um item do pedido
        public async Task AdicionarAsync(ItemPedido itemPedido)
        {
            await _context.itemPedidos.AddAsync(itemPedido);
            await _context.SaveChangesAsync();
        }

        // atualiza um item do pedido
        public async Task AtualizarAsync(ItemPedido itemPedido)
        {
            _context.itemPedidos.Update(itemPedido);
            await _context.SaveChangesAsync();
        }

        // remove um item do pedido
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

