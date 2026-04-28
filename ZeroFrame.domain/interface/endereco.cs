using ZeroFrame.domain.entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.domain.Interface
{
    // Interface que define o contrato do repositório de endereços.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IEnderecoRepository
    {
        Task<List<Endereco>> ObterTodosAsync();
        Task<Endereco?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Endereco endereco);
        Task AtualizarAsync(Endereco endereco);
        Task RemoverAsync(int id);
    }
}