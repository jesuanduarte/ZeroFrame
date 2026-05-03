using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS;

namespace ZeroFrame.Application.Interfaces
{
    public interface IVariacaoService
    {
        Task<List<VariacaoGetDto>> ObterTodosAsync();
        Task<VariacaoGetDto?> ObterPorIdAsync(int id);
        Task<List<VariacaoGetDto>> ObterPorProdutoIdAsync(int produtoId);
        Task<VariacaoGetDto> CriarAsync(VariacaoPostDto variacaoPostDto);
        Task AtualizarAsync(VariacaoPutDto variacaoPutDto);
        Task RemoverAsync(int id);
    }
}
