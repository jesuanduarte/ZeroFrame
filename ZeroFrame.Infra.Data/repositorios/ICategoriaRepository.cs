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
    public class CategoriaRepository : ICategoriaRepository
    {

        private readonly ApplicationDbContext _context;
       
        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        // Busca todos os registros dessa entidade no banco de dados.
        // Pode trazer também dados relacionados usando Include, se necessário.
        public async Task<List<Categoria>> ObterTodosAsync()
        {
            return await _context.categorias
                .Include(c => c.Produtos)
                .ToListAsync();
        }
        // Busca um único registro pelo Id.
        // Se não encontrar nenhum registro, retorna null
        public async Task<Categoria?> ObterPorIdAsync(int id)
        {
            return await _context.categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        // Adiciona um novo registro no banco de dados.
        // SaveChangesAsync confirma a gravação.
        public async Task AdicionarAsync(Categoria categoria)
        {
            await _context.categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        // Atualiza um registro já existente no banco de dados.
        // SaveChangesAsync confirma a alteração.
        public async Task AtualizarAsync(Categoria categoria)
        {
            _context.categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        // Remove um registro do banco de dados pelo Id.
        // Se o registro não existir, o método apenas encerra.
        public async Task RemoverAsync(int id)
        {
            var categoria = await _context.categorias.FindAsync(id);

            if (categoria is null)
                return;
            
          // Cancela um pedido alterando seu status.
          // Normalmente não remove o pedido do banco, apenas marca como cancelado.
            _context.categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}
    
