using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Endereco;
using ZeroFrame.Application.DTOS.Usuario;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{

    [ApiController]
    [Route("api/usuarios")]

    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEnderecoService _enderecoService;

        public UsuarioController(IUsuarioService usuarioService, IEnderecoService enderecoService)
        {
            _usuarioService = usuarioService;
            _enderecoService = enderecoService;
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

        // GET: api/usuarios/{usuarioId}/endereco
        // Busca o endereco do usuario.
        [HttpGet("{usuarioId:int}/endereco")]
        public async Task<ActionResult<EnderecoGetDto>> ObterEnderecoDoUsuario(int usuarioId)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            var endereco = await _enderecoService.ObterPorUsuarioIdAsync(usuarioId);

            if (endereco == null)
                return NotFound("Endereco nao encontrado.");

            return Ok(endereco);
        }

        // POST: api/usuarios/{usuarioId}/endereco
        // Cria um endereco para o usuario.
        [HttpPost("{usuarioId:int}/endereco")]
        public async Task<ActionResult<EnderecoGetDto>> CriarEnderecoDoUsuario(int usuarioId, EnderecoPostDto enderecoPostDto)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            if (enderecoPostDto.UsuarioId != usuarioId)
                return BadRequest("Id da rota diferente do UsuarioId do endereco.");

            var enderecoExistente = await _enderecoService.ObterPorUsuarioIdAsync(usuarioId);

            if (enderecoExistente != null)
                return BadRequest("Usuario ja possui endereco cadastrado.");

            var enderecoCriado = await _enderecoService.CriarAsync(enderecoPostDto);

            return CreatedAtAction(
                nameof(ObterEnderecoDoUsuario),
                new { usuarioId = usuarioId },
                enderecoCriado
            );
        }

        // PUT: api/usuarios/{usuarioId}/endereco/{enderecoId}
        // Atualiza o endereco do usuario.
        [HttpPut("{usuarioId:int}/endereco/{enderecoId:int}")]
        public async Task<ActionResult> AtualizarEnderecoDoUsuario(int usuarioId, int enderecoId, EnderecoPutDto enderecoPutDto)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            if (enderecoId != enderecoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do endereco.");

            if (enderecoPutDto.UsuarioId != usuarioId)
                return BadRequest("Id da rota diferente do UsuarioId do endereco.");

            var endereco = await _enderecoService.ObterPorIdAsync(enderecoId);

            if (endereco == null || endereco.UsuarioId != usuarioId)
                return NotFound("Endereco nao encontrado.");

            await _enderecoService.AtualizarAsync(enderecoPutDto);

            return NoContent();
        }

        // DELETE: api/usuarios/{usuarioId}/endereco/{enderecoId}
        // Remove o endereco do usuario.
        [HttpDelete("{usuarioId:int}/endereco/{enderecoId:int}")]
        public async Task<ActionResult> RemoverEnderecoDoUsuario(int usuarioId, int enderecoId)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            var endereco = await _enderecoService.ObterPorIdAsync(enderecoId);

            if (endereco == null || endereco.UsuarioId != usuarioId)
                return NotFound("Endereco nao encontrado.");

            await _enderecoService.RemoverAsync(enderecoId);

            return NoContent();
        }
    }
}
