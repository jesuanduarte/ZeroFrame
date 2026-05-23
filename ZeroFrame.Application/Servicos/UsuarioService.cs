using ZeroFrame.Application.DTOS.Endereco;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Usuario;
using ZeroFrame.Application.Exceptions;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Entidades;
using ZeroFrame.Domain.Interfaces;


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

            return MapearUsuarioGetDto(usuario);
        }

        // Busca um usuário pelo email.
        public async Task<UsuarioGetDto?> ObterPorEmailAsync(string email)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

            if (usuario == null)
                return null;

            return MapearUsuarioGetDto(usuario);
        }

        public async Task<List<UsuarioAdminGetDto>> ObterTodosAdminAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosAsync();

            return usuarios.Select(MapearUsuarioAdminGetDto).ToList();
        }

        public async Task<PagedResponse<UsuarioAdminGetDto>> ObterTodosAdminPaginadoAsync(PaginationParams paginationParams)
        {
            var resultado = await _usuarioRepository.ObterTodosPaginadoAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);

            var items = resultado.Items.Select(MapearUsuarioAdminGetDto).ToList();
            return PagedResponse<UsuarioAdminGetDto>.Create(items, resultado.TotalItems, paginationParams);
        }

        // Busca no banco de dados um usuário com o e-mail informado no login.
        public async Task<UsuarioLoginResponseDto?> AutenticarAsync(UsuarioLoginDto usuarioLoginDto)
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(usuarioLoginDto.Email.Trim());

            if (usuario == null || !usuario.Ativo)
                return null;

            if (!SenhaValida(usuarioLoginDto.Senha, usuario.Senha))
                return null;

            return new UsuarioLoginResponseDto
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo,
                Perfil = usuario.Perfil
            };
        }
        // Cria um novo usuário.
        public async Task<UsuarioGetDto> CriarAsync(UsuarioPostDto usuarioPostDto)
        {
            var emailNormalizado = usuarioPostDto.Email.Trim().ToLower();
            var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(emailNormalizado);

            if (usuarioExistente != null)
                throw new BadRequestException("Este e-mail ja esta cadastrado.");

            var usuario = new Usuario
            {
                Nome = usuarioPostDto.Nome.Trim(),
                Email = emailNormalizado,
                Senha = BCrypt.Net.BCrypt.HashPassword(usuarioPostDto.Senha),
                Telefone = usuarioPostDto.Telefone,
                Perfil = "Cliente",
                Ativo = true
            };

            await _usuarioRepository.CriarAsync(usuario);

            return MapearUsuarioGetDto(usuario);
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

        // mapeia os dados de um usuário para um DTO de resposta,
        // incluindo os endereços associados.
        private static UsuarioGetDto MapearUsuarioGetDto(Usuario usuario)
        {
            return new UsuarioGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Perfil = usuario.Perfil,
                Ativo = usuario.Ativo,
                Enderecos = usuario.Enderecos.Select(endereco => new EnderecoGetDto
                {
                    Id = endereco.Id,
                    Rua = endereco.Rua,
                    Numero = endereco.Numero,
                    Bairro = endereco.Bairro,
                    Cidade = endereco.Cidade,
                    Estado = endereco.Estado,
                    Cep = endereco.CEP,
                    Complemento = endereco.Complemento,
                    Ativo = endereco.Ativo,
                    UsuarioId = endereco.UsuarioId
                }).ToList()
            };
        }

        private static UsuarioAdminGetDto MapearUsuarioAdminGetDto(Usuario usuario)
        {
            return new UsuarioAdminGetDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo,
                Perfil = usuario.Perfil,
                QuantidadePedidos = usuario.Pedidos.Count
            };
        }

        // Verifica se a senha informada no login corresponde à senha armazenada no banco de dados.
        private static bool SenhaValida(string senhaInformada, string senhaArmazenada)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(senhaInformada, senhaArmazenada);
            }
            catch
            {
                // Usuarios antigos com senha em texto puro precisam ser recriados
                // ou ter a senha atualizada para hash.
                return false;
            }
        }
    }
}
