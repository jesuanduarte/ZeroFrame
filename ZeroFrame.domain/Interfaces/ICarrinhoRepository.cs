using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositório de carrinhos.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface ICarrinhoRepository
    {
        Task<List<Carrinho>> ObterTodosAsync();
        Task<Carrinho?> ObterPorIdAsync(int id);
        Task<Carrinho?> ObterAtivoPorUsuarioAsync(int usuarioId);
        Task AdicionarAsync(Carrinho carrinho);
        Task AtualizarAsync(Carrinho carrinho);
        Task RemoverAsync(int id);
    }
}