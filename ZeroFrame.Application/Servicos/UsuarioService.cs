using ZeroFrame.Application.DTOS.Usuario;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Ele faz a comunicação entre a Controller e o Repository.
    // Também realiza a conversão entre DTOs e Entidades.
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        // Recebe o repositório por injeção de dependência.
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // Busca um usuário pelo Id.
        public async Task<UsuarioGetDto?> ObterPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);

            if (usuario == null)
                return null;

            return new UsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo
            };
        }

        // Busca um usuário pelo email.
        public async Task<UsuarioGetDto?> ObterPorEmailAsync(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

            if (usuario == null)
                return null;

            return new UsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo
            };
        }
        public async Task<UsuarioLoginResponseDto?> AutenticarAsync(UsuarioLoginDto usuarioLoginDto)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(usuarioLoginDto.Email.Trim());

            if (usuario == null || !usuario.Ativo)
                return null;

            if (usuario.Senha != usuarioLoginDto.Senha)
                return null;

            return new UsuarioLoginResponseDto
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo
            };
        }
        public async Task<UsuarioGetDto> CriarCadastroSimplesAsync(UsuarioCadastroSimplesDto usuarioCadastroSimplesDto)
        {
            if (usuarioCadastroSimplesDto.Senha != usuarioCadastroSimplesDto.ConfirmarSenha)
                throw new InvalidOperationException("Senha e confirmacao de senha nao conferem.");

            var email = usuarioCadastroSimplesDto.Email.Trim();
            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(email);

            if (usuarioExistente != null)
                throw new InvalidOperationException("Ja existe um usuario cadastrado com este email.");

            var nomePadrao = email.Split('@')[0];

            var usuario = new Usuario
            {
                Nome = nomePadrao,
                Email = email,
                Senha = usuarioCadastroSimplesDto.Senha,
                Telefone = "Nao informado",
                Ativo = true
            };

            await _usuarioRepository.CriarAsync(usuario);

            return new UsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo
            };
        }

        // Cria um novo usuário.
        public async Task<UsuarioGetDto> CriarAsync(UsuarioPostDto usuarioPostDto)
        {
            var usuario = new Usuario
            {
                Nome = usuarioPostDto.Nome,
                Email = usuarioPostDto.Email,
                Senha = usuarioPostDto.Senha,
                Telefone = usuarioPostDto.Telefone,
                Ativo = true
            };

            await _usuarioRepository.CriarAsync(usuario);

            return new UsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo
            };
        }

        // Atualiza os dados básicos de um usuário.
        public async Task AtualizarAsync(UsuarioPutDto usuarioPutDto)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioPutDto.Id);

            if (usuario == null)
                return;

            usuario.Nome = usuarioPutDto.Nome;
            usuario.Telefone = usuarioPutDto.Telefone;

            await _usuarioRepository.AtualizarAsync(usuario);
        }

        // Remove um usuário pelo Id.
        public async Task RemoverAsync(int id)
        {
            await _usuarioRepository.RemoverAsync(id);
        }
    }
}


