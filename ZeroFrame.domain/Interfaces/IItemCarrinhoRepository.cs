using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositório de itens de carrinho.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IItemCarrinhoRepository
    {
        Task<List<ItemCarrinho>> ObterTodosAsync();
        Task<ItemCarrinho?> ObterPorIdAsync(int id);
        Task<List<ItemCarrinho>> ObterPorCarrinhoAsync(int carrinhoId);
        Task<ItemCarrinho?> ObterPorCarrinhoEVariacaoAsync(int carrinhoId, int variacaoProdutoId);
        Task AdicionarAsync(ItemCarrinho itemCarrinho);
        Task AtualizarAsync(ItemCarrinho itemCarrinho);
        Task RemoverAsync(int id);
    }
}