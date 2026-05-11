using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Filtros;
using ZeroFrame.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using ZeroFrame.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Infra.Data.Repositorios
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
            return await ObterTodosAsync(new ProdutoFiltro());
        }

        public async Task<List<Produto>> ObterTodosAsync(ProdutoFiltro filtro)
        {
            var query = _context.produtos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .Include(p => p.VariacoesProdutos)
                .AsQueryable();

            query = AplicarFiltros(query, filtro);

            return await query.ToListAsync();
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

        //Aplica uma série de filtros dinâmicos a uma consulta de produtos.
        //
        private static IQueryable<Produto> AplicarFiltros(IQueryable<Produto> query, ProdutoFiltro filtro)
        {
            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                var busca = filtro.Busca.Trim().ToLower();

                query = query.Where(produto =>
                    produto.Nome.ToLower().Contains(busca)
                    || produto.Descricao.ToLower().Contains(busca)
                    || (produto.Categoria != null && produto.Categoria.Nome.ToLower().Contains(busca))
                    || produto.Marca.ToLower().Contains(busca)
                    || produto.Origem.ToLower().Contains(busca)
                    || (produto.Marca == string.Empty
                        && ((busca == "nike" && (produto.Nome.ToLower().Contains("nike") || produto.Nome.ToLower().Contains("jordan")))
                            || (busca == "adidas" && produto.Nome.ToLower().Contains("adidas"))
                            || ((busca == "levis" || busca == "levi's") && (produto.Nome.ToLower().Contains("levis") || produto.Nome.ToLower().Contains("levi")))
                            || (busca == "zero frame"
                                && !produto.Nome.ToLower().Contains("nike")
                                && !produto.Nome.ToLower().Contains("jordan")
                                && !produto.Nome.ToLower().Contains("adidas")
                                && !produto.Nome.ToLower().Contains("levis")
                                && !produto.Nome.ToLower().Contains("levi"))))
                    || (produto.Origem == string.Empty
                        && ((busca == "multimarcas"
                                && (produto.Nome.ToLower().Contains("nike")
                                    || produto.Nome.ToLower().Contains("jordan")
                                    || produto.Nome.ToLower().Contains("adidas")
                                    || produto.Nome.ToLower().Contains("levis")
                                    || produto.Nome.ToLower().Contains("levi")))
                            || (busca == "original"
                                && !produto.Nome.ToLower().Contains("nike")
                                && !produto.Nome.ToLower().Contains("jordan")
                                && !produto.Nome.ToLower().Contains("adidas")
                                && !produto.Nome.ToLower().Contains("levis")
                                && !produto.Nome.ToLower().Contains("levi")))));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Categoria))
            {
                var categoria = filtro.Categoria.Trim().ToLower();

                query = int.TryParse(categoria, out var categoriaId)
                    ? query.Where(produto => produto.CategoriaId == categoriaId)
                    : query.Where(produto => produto.Categoria != null
                        && produto.Categoria.Nome.ToLower().Contains(categoria));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Marca))
            {
                var marca = filtro.Marca.Trim().ToLower();

                query = query.Where(produto =>
                    produto.Marca.ToLower().Contains(marca)
                    || (produto.Marca == string.Empty
                        && ((marca == "nike" && (produto.Nome.ToLower().Contains("nike") || produto.Nome.ToLower().Contains("jordan")))
                            || (marca == "adidas" && produto.Nome.ToLower().Contains("adidas"))
                            || ((marca == "levis" || marca == "levi's") && (produto.Nome.ToLower().Contains("levis") || produto.Nome.ToLower().Contains("levi")))
                            || (marca == "zero frame"
                                && !produto.Nome.ToLower().Contains("nike")
                                && !produto.Nome.ToLower().Contains("jordan")
                                && !produto.Nome.ToLower().Contains("adidas")
                                && !produto.Nome.ToLower().Contains("levis")
                                && !produto.Nome.ToLower().Contains("levi")))));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Origem))
            {
                var origem = filtro.Origem.Trim().ToLower();

                query = query.Where(produto =>
                    produto.Origem.ToLower().Contains(origem)
                    || (produto.Origem == string.Empty
                        && ((origem == "multimarcas"
                                && (produto.Nome.ToLower().Contains("nike")
                                    || produto.Nome.ToLower().Contains("jordan")
                                    || produto.Nome.ToLower().Contains("adidas")
                                    || produto.Nome.ToLower().Contains("levis")
                                    || produto.Nome.ToLower().Contains("levi")))
                            || (origem == "original"
                                && !produto.Nome.ToLower().Contains("nike")
                                && !produto.Nome.ToLower().Contains("jordan")
                                && !produto.Nome.ToLower().Contains("adidas")
                                && !produto.Nome.ToLower().Contains("levis")
                                && !produto.Nome.ToLower().Contains("levi")))));
            }

            if (filtro.PrecoMin.HasValue)
            {
                query = query.Where(produto => produto.Preco >= filtro.PrecoMin.Value);
            }

            if (filtro.PrecoMax.HasValue)
            {
                query = query.Where(produto => produto.Preco <= filtro.PrecoMax.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Tamanho))
            {
                var tamanho = filtro.Tamanho.Trim().ToLower();

                query = query.Where(produto => produto.VariacoesProdutos
                    .Any(variacao => variacao.Tamanho.ToLower().Contains(tamanho)));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Cor))
            {
                var cor = filtro.Cor.Trim().ToLower();

                query = query.Where(produto => produto.VariacoesProdutos
                    .Any(variacao => variacao.Cor.ToLower().Contains(cor)));
            }

            return query;
        }

    }
}
