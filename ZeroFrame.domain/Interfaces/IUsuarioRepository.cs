using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositório de usuários.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<List<Usuario>> ObterTodosAsync();
        Task<(List<Usuario> Items, int TotalItems)> ObterTodosPaginadoAsync(int pageNumber, int pageSize);
        Task<Usuario?> ObterPorIdAsync(int id);
        Task AtualizarAsync(Usuario usuario);
        Task CriarAsync(Usuario usuario);
        Task RemoverAsync(int id);
    }
}
