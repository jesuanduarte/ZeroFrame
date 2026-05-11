using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.Domain.Interfaces
{
    // Interface que define o contrato do repositório de variação de produtos.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IVariacaoRepository
    {
        Task<List<VariacaoProdutos>> ObterTodosAsync();
        Task<VariacaoProdutos?> ObterPorIdAsync(int id);
        Task<List<VariacaoProdutos>> ObterPorProdutoIdAsync(int produtoId);
        Task AdicionarAsync(VariacaoProdutos variacaoProdutos);
        Task AtualizarAsync(VariacaoProdutos variacaoProdutos);
        Task RemoverAsync(int id);
    }
}