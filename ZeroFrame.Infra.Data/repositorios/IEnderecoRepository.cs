using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.BDconexao;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.repositorios
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly ApplicationDbContext _context;

        public EnderecoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Endereco>> ObterTodosAsync()
        {
            return await _context.enderecos
                .Include(e => e.Usuario)
                .ToListAsync();
        }

        public async Task<Endereco?> ObterPorIdAsync(int id)
        {
            return await _context.enderecos
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AdicionarAsync(Endereco endereco)
        {
            await _context.enderecos.AddAsync(endereco);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Endereco endereco)
        {
            _context.enderecos.Update(endereco);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var endereco = await _context.enderecos.FindAsync(id);

            if (endereco is null)
                return;

            _context.enderecos.Remove(endereco);
            await _context.SaveChangesAsync();
        }
    }
}