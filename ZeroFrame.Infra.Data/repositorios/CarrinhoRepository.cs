using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.Repositorios
{
    public class CarrinhoRepository : ICarrinhoRepository
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Busca todos os carrinhos cadastrados no banco,

        public async Task<List<Carrinho>> ObterTodosAsync()
        {
            return await _context.carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .ToListAsync();
        }
        // Busca um carrinho específico pelo Id,
        public async Task<Carrinho?> ObterPorIdAsync(int id)
        {
            return await _context.carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Busca o carrinho ativo de um usuário específico,
        // incluindo os itens e os dados dos produtos.
        public async Task<Carrinho?> ObterAtivoPorUsuarioAsync(int usuarioId)
        {
            return await _context.carrinhos
                .Include(c => c.Usuario)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.VariacaoProduto)
                        .ThenInclude(v => v!.Produto)
                            .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Ativo);
        }

        // Adiciona um novo carrinho no banco de dados
        // e salva a alteração.
        public async Task AdicionarAsync(Carrinho carrinho)
        {
            await _context.carrinhos.AddAsync(carrinho);
            await _context.SaveChangesAsync();
        }

        // Atualiza os dados de um carrinho existente
        // e salva a alteração no banco.
        public async Task AtualizarAsync(Carrinho carrinho)
        {
            _context.carrinhos.Update(carrinho);
            await _context.SaveChangesAsync();
        }

        // Busca um carrinho pelo Id.
        // Se existir, remove do banco e salva a alteração.
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


