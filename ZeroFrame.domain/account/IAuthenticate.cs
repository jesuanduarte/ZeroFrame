using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Domain.account
{
    public interface IAuthenticate
    {
        // Gera o token JWT do usuário autenticado
        string GenerateToken(int id, string email, string role);

        // Busca um usuário pelo email
        Task<Usuario?> GetUser(string email);
        Task<bool> userExists(string email);

    }
}
