using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Usuario;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    
    [ApiController]
    [Route("api/usuarios")]

    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // POST: api/usuarios
        // Cria um novo usuário.
        [HttpPost]
        public async Task<ActionResult<UsuarioGetDto>> CriarUsuario(UsuarioPostDto usuarioPostDto)
        {
            // Envia os dados para o serviço criar o usuário.
            var usuarioCriado = await _usuarioService.CriarAsync(usuarioPostDto);

            // Retorna 201 Created informando que o usuário foi criado com sucesso.
            return CreatedAtAction(
                nameof(ObterUsuarioPorId),
                new { id = usuarioCriado.Id },
                usuarioCriado
            );
        }
        // POST: api/usuarios/cadastro-simples
        // Cria um usuário usando os campos disponíveis na tela atual de cadastro.
        [HttpPost("cadastro-simples")]
        public async Task<ActionResult<UsuarioGetDto>> CadastroSimplesUsuario(UsuarioCadastroSimplesDto usuarioCadastroSimplesDto)
        {
            try
            {
                var usuarioCriado = await _usuarioService.CriarCadastroSimplesAsync(usuarioCadastroSimplesDto);

                return CreatedAtAction(
                    nameof(ObterUsuarioPorId),
                    new { id = usuarioCriado.Id },
                    usuarioCriado
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // POST: api/usuarios/login
        // Autentica um usuário pelo email e senha.
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioLoginResponseDto>> LoginUsuario(UsuarioLoginDto usuarioLoginDto)
        {
            var usuarioAutenticado = await _usuarioService.AutenticarAsync(usuarioLoginDto);

            if (usuarioAutenticado == null)
                return Unauthorized("Email ou senha invalidos.");

            return Ok(usuarioAutenticado);
        }

        // PUT: api/usuarios/{id}
        // Atualiza os dados de um usuário existente.
        [HttpPut("{id:int}")]
        public async Task<ActionResult> AtualizarUsuario(int id, UsuarioPutDto usuarioPutDto)
        {
            // Verifica se o Id da rota é igual ao Id enviado no corpo da requisição.
            if (id != usuarioPutDto.Id)
                return BadRequest("Id da rota diferente do Id do usuario.");

            // Busca o usuário antes de atualizar, para confirmar se ele existe.
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            // Caso o usuário não exista, retorna 404 Not Found.
            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            // Atualiza o usuário.
            await _usuarioService.AtualizarAsync(usuarioPutDto);

            // Retorna 204 No Content indicando que a atualização foi feita com sucesso.
            return NoContent();
        }

        // GET: api/usuarios/{id}
        // Busca um usuário específico pelo Id.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioGetDto>> ObterUsuarioPorId(int id)
        {
            // Busca o usuário pelo Id informado na rota.
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            // Caso o usuário não exista, retorna 404 Not Found.
            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            // Retorna o usuário encontrado.
            return Ok(usuario);
        }

        // DELETE: api/usuarios/{id}
        // Remove um usuário existente.
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> RemoverUsuario(int id)
        {
            // Busca o usuário antes de remover, para confirmar se ele existe.
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            // Caso o usuário não exista, retorna 404 Not Found.
            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            // Remove o usuário.
            await _usuarioService.RemoverAsync(id);

            // Retorna 204 No Content indicando que a remoção foi feita com sucesso.
            return NoContent();
        }
    }
}

