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
    public class VariacaoRepository : IVariacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public VariacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VariacaoProdutos>> ObterTodosAsync()
        {
            return await _context.variacaoprodutos
                .Include(v => v.Produto)
                .ToListAsync();
        }

        public async Task<VariacaoProdutos?> ObterPorIdAsync(int id)
        {
            return await _context.variacaoprodutos
                .Include(v => v.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task AdicionarAsync(VariacaoProdutos variacaoProdutos)
        {
            await _context.variacaoprodutos.AddAsync(variacaoProdutos);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(VariacaoProdutos variacaoProdutos)
        {
            _context.variacaoprodutos.Update(variacaoProdutos);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var variacao = await _context.variacaoprodutos.FindAsync(id);

            if (variacao is null)
                return;

            _context.variacaoprodutos.Remove(variacao);
            await _context.SaveChangesAsync();
        }
    }
}

  