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
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Busca um usuário pelo email.
        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            var emailNormalizado = email.Trim().ToLower();

            return await _context.Usuarios
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado);
        }

        public async Task<List<Usuario>> ObterTodosAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Pedidos)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(List<Usuario> Items, int TotalItems)> ObterTodosPaginadoAsync(int pageNumber, int pageSize)
        {
            var query = _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Pedidos)
                .AsQueryable();

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        // Busca um usuário pelo ID.
        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Busca todos os usuários.
        public async Task CriarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        // Atualiza um usuário existente.
        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        // Remove um usuário pelo ID. 
        public async Task RemoverAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario is null)
                return;

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }
    }
}

