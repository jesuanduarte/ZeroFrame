using Microsoft.EntityFrameworkCore;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;
using ZeroFrame.Infra.Data.Context;

namespace ZeroFrame.Infra.Data.Repositorios
{
    public class AvaliacaoProdutoRepository : IAvaliacaoProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public AvaliacaoProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AvaliacaoProduto> CriarAsync(AvaliacaoProduto avaliacao)
        {
            await _context.AvaliacoesProdutos.AddAsync(avaliacao);
            await _context.SaveChangesAsync();
            return avaliacao;
        }

        public async Task AtualizarAsync(AvaliacaoProduto avaliacao)
        {
            _context.AvaliacoesProdutos.Update(avaliacao);
            await _context.SaveChangesAsync();
        }

        public async Task<AvaliacaoProduto?> ObterPorIdAsync(int id)
        {
            return await _context.AvaliacoesProdutos
                .Include(a => a.Usuario)
                .Include(a => a.Produto)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<AvaliacaoProduto?> ObterAtivaPorUsuarioEProdutoAsync(int usuarioId, int produtoId)
        {
            return await _context.AvaliacoesProdutos
                .Include(a => a.Usuario)
                .Include(a => a.Produto)
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId
                    && a.ProdutoId == produtoId
                    && a.Ativo);
        }

        public async Task<List<AvaliacaoProduto>> ListarAtivasPorProdutoAsync(int produtoId)
        {
            return await _context.AvaliacoesProdutos
                .AsNoTracking()
                .Include(a => a.Usuario)
                .Include(a => a.Produto)
                .Where(a => a.ProdutoId == produtoId && a.Ativo)
                .OrderByDescending(a => a.DataCriacao)
                .ToListAsync();
        }

        public async Task<(List<AvaliacaoProduto> Items, int TotalItems)> ListarAtivasPorProdutoPaginadoAsync(
            int produtoId,
            int pageNumber,
            int pageSize)
        {
            var query = _context.AvaliacoesProdutos
                .AsNoTracking()
                .Include(a => a.Usuario)
                .Include(a => a.Produto)
                .Where(a => a.ProdutoId == produtoId && a.Ativo)
                .OrderByDescending(a => a.DataCriacao);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalItems);
        }

        public async Task DesativarAsync(int id)
        {
            var avaliacao = await _context.AvaliacoesProdutos.FindAsync(id);

            if (avaliacao is null)
                return;

            avaliacao.Ativo = false;
            avaliacao.DataAtualizacao = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task ApagarAsync(int id)
        {
            var avaliacao = await _context.AvaliacoesProdutos.FindAsync(id);

            if (avaliacao is null)
                return;

            _context.AvaliacoesProdutos.Remove(avaliacao);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.AvaliacoesProdutos.AnyAsync(a => a.Id == id);
        }

        public async Task<decimal> CalcularMediaAvaliacoesAsync(int produtoId)
        {
            var avaliacoes = _context.AvaliacoesProdutos
                .AsNoTracking()
                .Where(a => a.ProdutoId == produtoId && a.Ativo);

            if (!await avaliacoes.AnyAsync())
                return 0m;

            return await avaliacoes.AverageAsync(a => a.Nota);
        }

        public async Task<List<AvaliacaoProduto>> BuscarResumoAvaliacoesAsync(int produtoId)
        {
            return await _context.AvaliacoesProdutos
                .AsNoTracking()
                .Where(a => a.ProdutoId == produtoId && a.Ativo)
                .ToListAsync();
        }
    }
}
