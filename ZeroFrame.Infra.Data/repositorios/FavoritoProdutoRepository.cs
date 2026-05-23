using Microsoft.EntityFrameworkCore;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;
using ZeroFrame.Infra.Data.Context;

namespace ZeroFrame.Infra.Data.Repositorios
{
    public class FavoritoProdutoRepository : IFavoritoProdutoRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoritoProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FavoritoProduto>> ObterPorUsuarioAsync(int usuarioId)
        {
            return await _context.FavoritosProdutos
                .AsNoTracking()
                .Include(f => f.Produto)
                    .ThenInclude(p => p!.Categoria)
                .Where(f => f.UsuarioId == usuarioId)
                .OrderByDescending(f => f.DataCriacao)
                .ToListAsync();
        }

        public async Task<FavoritoProduto?> ObterPorUsuarioEProdutoAsync(int usuarioId, int produtoId)
        {
            return await _context.FavoritosProdutos
                .Include(f => f.Produto)
                    .ThenInclude(p => p!.Categoria)
                .FirstOrDefaultAsync(f => f.UsuarioId == usuarioId && f.ProdutoId == produtoId);
        }

        public async Task AdicionarAsync(FavoritoProduto favorito)
        {
            await _context.FavoritosProdutos.AddAsync(favorito);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(FavoritoProduto favorito)
        {
            _context.FavoritosProdutos.Remove(favorito);
            await _context.SaveChangesAsync();
        }
    }
}
