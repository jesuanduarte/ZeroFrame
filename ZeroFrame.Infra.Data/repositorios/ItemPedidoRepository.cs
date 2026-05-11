using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.Repositorios
{
    public class ItemPedidoRepository : IItemPedidoRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemPedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Busca um item especifico do pedido pelo Id do proprio ItemPedido.
        public async Task<ItemPedido?> ObterPorIdAsync(int id)
        {
            return await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // // Busca todos os itens que pertencem a um pedido especifico.
        public async Task<List<ItemPedido>> ObterPorPedidoAsync(int pedidoId)
        {
            return await _context.itemPedidos
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
                .Where(i => i.PedidoId == pedidoId)
                .ToListAsync();
        }

        // busca todos os itens do pedido
        public async Task<List<ItemPedido>> ObterTodosAsync()
        {
            return await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
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

