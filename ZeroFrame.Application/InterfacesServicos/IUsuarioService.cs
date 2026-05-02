using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Usuario;

namespace ZeroFrame.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioGetDto?> ObterPorIdAsync(int id);
        Task<UsuarioGetDto?> ObterPorEmailAsync(string email);
        Task<UsuarioLoginResponseDto?> AutenticarAsync(UsuarioLoginDto usuarioLoginDto);
        Task<UsuarioGetDto> CriarCadastroSimplesAsync(UsuarioCadastroSimplesDto usuarioCadastroSimplesDto);
        Task<UsuarioGetDto> CriarAsync(UsuarioPostDto usuarioPostDto);
        Task AtualizarAsync(UsuarioPutDto usuarioPutDto);
        Task RemoverAsync(int id);
    }
}
 