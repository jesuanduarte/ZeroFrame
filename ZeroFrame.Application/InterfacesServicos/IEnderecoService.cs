using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Endereco;

namespace ZeroFrame.Application.Interfaces
{
    public interface IEnderecoService
    {
        Task<List<EnderecoGetDto>> ObterTodosAsync();
        Task<EnderecoGetDto?> ObterPorIdAsync(int id);
        Task<EnderecoGetDto?> ObterPorUsuarioIdAsync(int usuarioId);
        Task<List<EnderecoGetDto>> ObterTodosPorUsuarioIdAsync(int usuarioId);
        Task<EnderecoGetDto> CriarAsync(EnderecoPostDto enderecoPostDto);
        Task AtualizarAsync(EnderecoPutDto enderecoPutDto);
        Task RemoverAsync(int id);
    }
}
