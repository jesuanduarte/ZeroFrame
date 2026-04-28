using ZeroFrame.domain.entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.domain.Interface
{
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