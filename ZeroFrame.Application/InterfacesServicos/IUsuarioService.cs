using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Usuario;

namespace ZeroFrame.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioGetDto?> ObterPorIdAsync(int id);
        Task<List<UsuarioAdminGetDto>> ObterTodosAdminAsync();
        Task<PagedResponse<UsuarioAdminGetDto>> ObterTodosAdminPaginadoAsync(PaginationParams paginationParams);
        Task<UsuarioGetDto?> ObterPorEmailAsync(string email);
        Task<UsuarioLoginResponseDto?> AutenticarAsync(UsuarioLoginDto usuarioLoginDto);
        Task<UsuarioGetDto> CriarAsync(UsuarioPostDto usuarioPostDto);
        Task AtualizarAsync(UsuarioPutDto usuarioPutDto);
        Task RemoverAsync(int id);
    }
}
