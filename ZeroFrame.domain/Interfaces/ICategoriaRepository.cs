using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositório de categorias.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface ICategoriaRepository
    {
        Task<List<Categoria>> ObterTodosAsync();
        Task<Categoria?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Categoria categoria);
        Task AtualizarAsync(Categoria categoria);
        Task RemoverAsync(int id);
    }
}
