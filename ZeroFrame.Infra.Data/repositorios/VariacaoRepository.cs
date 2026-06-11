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
    public class VariacaoRepository : IVariacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public VariacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // retorna uma lista de todas as variações dos produtos
        public async Task<List<VariacaoProdutos>> ObterTodosAsync()
        {
            return await _context.variacaoprodutos
                .Include(v => v.Produto)
                    .ThenInclude(p => p!.Categoria)
                .ToListAsync();
        }

        // retorna uma variação de produto específica com base no ID fornecido
        public async Task<VariacaoProdutos?> ObterPorIdAsync(int id)
        {
            return await _context.variacaoprodutos
                .Include(v => v.Produto)
                    .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        // retorna uma lista de variações de produtos associadas a um produto específico, 
        public async Task<List<VariacaoProdutos>> ObterPorProdutoIdAsync(int produtoId)
        {
            return await _context.variacaoprodutos
                .Include(v => v.Produto)
                    .ThenInclude(p => p!.Categoria)
                .Where(v => v.ProdutoId == produtoId)
                .ToListAsync();
        }

        // adiciona uma nova variação de produto ao banco de dados
        public async Task AdicionarAsync(VariacaoProdutos variacaoProdutos)
        {
            await _context.variacaoprodutos.AddAsync(variacaoProdutos);
            await _context.SaveChangesAsync();
        }

        //  atualiza uma variação de produto existente no banco de dados
        public async Task AtualizarAsync(VariacaoProdutos variacaoProdutos)
        {
            _context.variacaoprodutos.Update(variacaoProdutos);
            await _context.SaveChangesAsync();
        }

        // remove uma variação de produto do banco de dados com base no ID fornecido
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

  