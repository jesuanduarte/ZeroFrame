using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.BDconexao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.repositorios
{
    public class ItemCarrinhoRepository : IItemCarrinhoRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemCarrinhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemCarrinho>> ObterTodosAsync()
        {
            return await _context.item_Carrinhos
                .Include(i => i.Carrinho)
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
                .ToListAsync();
        }

        public async Task<ItemCarrinho?> ObterPorIdAsync(int id)
        {
            return await _context.item_Carrinhos
                .Include(i => i.Carrinho)
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<ItemCarrinho>> ObterPorCarrinhoAsync(int carrinhoId)
        {
            return await _context.item_Carrinhos
                .Include(i => i.Carrinho)
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
                .Where(i => i.CarrinhoId == carrinhoId)
                .ToListAsync();
        }

        public async Task<ItemCarrinho?> ObterPorCarrinhoEVariacaoAsync(int carrinhoId, int variacaoProdutoId)
        {
            return await _context.item_Carrinhos
                .Include(i => i.Carrinho)
                .Include(i => i.VariacaoProduto)
                    .ThenInclude(v => v!.Produto)
                        .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(i =>
                    i.CarrinhoId == carrinhoId &&
                    i.VariacaoProdutoId == variacaoProdutoId);
        }

        public async Task AdicionarAsync(ItemCarrinho itemCarrinho)
        {
            await _context.item_Carrinhos.AddAsync(itemCarrinho);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(ItemCarrinho itemCarrinho)
        {
            _context.item_Carrinhos.Update(itemCarrinho);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var itemCarrinho = await _context.item_Carrinhos.FindAsync(id);

            if (itemCarrinho is null)
                return;

            _context.item_Carrinhos.Remove(itemCarrinho);
            await _context.SaveChangesAsync();
        }
    }
}
