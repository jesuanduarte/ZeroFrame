using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.BDconexao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.repositorios
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //busca todos os produtos
        public async Task<List<Produto>> ObterTodosAsync()
        {
            return await _context.produtos
                .Include(p => p.Categoria)
                .Include(p => p.VariacoesProdutos)
                .ToListAsync();
        }

        // busca um produto por id
        public async Task<Produto?> ObterPorIdAsync(int id)
        {
            return await _context.produtos
                .Include(p => p.Categoria)
                .Include(p => p.VariacoesProdutos)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // busca produtos por categoria
        public async Task AdicionarAsync(Produto produto)
        {
            await _context.produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
        }

        // atualiza um produto
        public async Task AtualizarAsync(Produto produto)
        {
            _context.produtos.Update(produto);
            await _context.SaveChangesAsync();
        }

        // remove um produto
        public async Task RemoverAsync(int id)
        {
            var produto = await _context.produtos.FindAsync(id);

            if (produto is null)
                return;

            _context.produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }
    }
}