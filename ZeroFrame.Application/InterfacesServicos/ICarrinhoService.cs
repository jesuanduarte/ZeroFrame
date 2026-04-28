using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS;

namespace ZeroFrame.Application.Interfaces
{
    public interface ICarrinhoService
    {
        Task<List<CarrinhoGetDto>> ObterTodosAsync();
        Task<CarrinhoGetDto?> ObterPorIdAsync(int id);
        Task<CarrinhoGetDto> ObterOuCriarAtivoPorUsuarioAsync(int usuarioId);
        Task<CarrinhoGetDto> CriarAsync(CarrinhoPostDto carrinhoPostDto);
        Task AtualizarAsync(CarrinhoPutDto carrinhoPutDto);
        Task RemoverAsync(int id);
    }
}
