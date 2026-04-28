using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.BDconexao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.repositorios
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Carrinho>> ObterTodosAsync()
        {
            return await _context.carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.Itens)
                .ToListAsync();
        }

        public async Task<Carrinho?> ObterPorIdAsync(int id)
        {
            return await _context.carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Carrinho?> ObterAtivoPorUsuarioAsync(int usuarioId)
        {
            return await _context.carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Ativo);
        }

        public async Task AdicionarAsync(Carrinho carrinho)
        {
            await _context.carrinhos.AddAsync(carrinho);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Carrinho carrinho)
        {
            _context.carrinhos.Update(carrinho);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var carrinho = await _context.carrinhos.FindAsync(id);

            if (carrinho is null)
                return;

            _context.carrinhos.Remove(carrinho);
            await _context.SaveChangesAsync();
        }
    }
}


