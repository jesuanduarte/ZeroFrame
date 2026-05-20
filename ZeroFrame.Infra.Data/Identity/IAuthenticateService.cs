using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Account;
using ZeroFrame.Infra.Data.Context;

namespace ZeroFrame.Infra.Data.Identity
{
    public class AuthenticateService : IAuthenticate
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticateService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // metodo para gerar um token JWT para um usuário autenticado. O token inclui informações do usuário,
        // incluindo suas reivindicações e informações de segurança.
        public string GenerateToken(int id, string email, string role)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email.ToLower()),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var privatekey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!)
            );

            var credentials = new SigningCredentials(
                privatekey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // metodo para obter um usuário do banco de dados com base no email fornecido. 
        public async Task<Usuario?> GetUser(string email)
        {
            var emailNormalizado = email.Trim().ToLower();

            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNormalizado);
        }

        // metodo para verificar se um usuário com o email fornecido já existe no banco de dados.
        public async Task<bool> userExists(string email)
        {
            var emailNormalizado = email.Trim().ToLower();

            return await _context.Usuarios
                .AnyAsync(u => u.Email.ToLower() == emailNormalizado);
        }
    }
}
