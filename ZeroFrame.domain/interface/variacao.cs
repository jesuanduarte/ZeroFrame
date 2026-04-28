using ZeroFrame.domain.entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZeroFrame.domain.Interface
{
    // Interface que define o contrato do repositório de variação de produtos.
    // Ela estabelece quais operações deverão existir na implementação.
    public interface IVariacaoRepository
    {
        Task<List<VariacaoProdutos>> ObterTodosAsync();
        Task<VariacaoProdutos?> ObterPorIdAsync(int id);
        Task AdicionarAsync(VariacaoProdutos variacaoProdutos);
        Task AtualizarAsync(VariacaoProdutos variacaoProdutos);
        Task RemoverAsync(int id);
    }
}