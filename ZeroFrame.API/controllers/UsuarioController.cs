using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.Endereco;
using ZeroFrame.Application.DTOS.Usuario;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Account;
using ZeroFrame.Infra.Data.Identity;

namespace ZeroFrame.API.Controllers
{

    [ApiController]
    [Route("api/usuarios")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]

    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEnderecoService _enderecoService;
        private readonly IAuthenticate _authenticateService;

        public UsuarioController(
            IUsuarioService usuarioService,
            IEnderecoService enderecoService,
            IAuthenticate authenticateService)
        {
            _usuarioService = usuarioService;
            _enderecoService = enderecoService;
            _authenticateService = authenticateService;
        }
       
        // POST: api/usuarios
        // Cria um novo usuário.
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UsuarioGetDto>> CriarUsuario(UsuarioPostDto usuarioPostDto)
        {
            var usuarioCriado = await _usuarioService.CriarAsync(usuarioPostDto);

            var token = _authenticateService.GenerateToken(
                usuarioCriado.Id,
                usuarioCriado.Email,
                usuarioCriado.Perfil
            );

            return CreatedAtAction(
                nameof(ObterUsuarioPorId),
                new { id = usuarioCriado.Id },
                new
                {
                    usuarioCriado.Id,
                    usuarioCriado.Email,
                    usuarioCriado.Perfil,
                    Token = token
                }
            );
        }

        // POST: api/usuarios/login
        // Autentica um usuário pelo email e senha.
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioLoginResponseDto>> LoginUsuario(UsuarioLoginDto usuarioLoginDto)
        {
            var usuarioAutenticado = await _usuarioService.AutenticarAsync(usuarioLoginDto);

            if (usuarioAutenticado == null)
                return Unauthorized("Email ou senha invalidos.");

            var token = _authenticateService.GenerateToken(
                usuarioAutenticado.UsuarioId,
                usuarioAutenticado.Email,
                usuarioAutenticado.Perfil
            );

            usuarioAutenticado.Token = token;

            return Ok(usuarioAutenticado);
        }

        // PUT: api/usuarios/{id}
        // Apenas Administrador pode atualizar usuário.
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> AtualizarUsuario(int id, UsuarioPutDto usuarioPutDto)
        {
            if (id != usuarioPutDto.Id)
                return BadRequest("Id da rota diferente do Id do usuario.");

            var usuario = await _usuarioService.ObterPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            await _usuarioService.AtualizarAsync(usuarioPutDto);

            return NoContent();
        }

        // GET: api/usuarios/{id}
        // Apenas Administrador pode buscar usuário por Id.
        [Authorize(Roles = "Administrador")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioGetDto>> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            return Ok(usuario);
        }

        // DELETE: api/usuarios/{id}
        // Apenas Administrador pode remover usuário.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> RemoverUsuario(int id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            await _usuarioService.RemoverAsync(id);

            return NoContent();
        }

        // GET: api/usuarios/{usuarioId}/endereco
        // Cliente e Administrador podem buscar endereço.
        [Authorize(Roles = "Cliente,Administrador")]
        [HttpGet("{usuarioId:int}/endereco")]
        public async Task<ActionResult<EnderecoGetDto>> ObterEnderecoDoUsuario(int usuarioId)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            var endereco = await _enderecoService.ObterPorUsuarioIdAsync(usuarioId);

            if (endereco == null)
                return NotFound("Endereco nao encontrado.");

            return Ok(endereco);
        }

        // POST: api/usuarios/{usuarioId}/endereco
        // Cliente e Administrador podem criar endereço.
        [Authorize(Roles = "Cliente,Administrador")]
        [HttpPost("{usuarioId:int}/endereco")]
        public async Task<ActionResult<EnderecoGetDto>> CriarEnderecoDoUsuario(int usuarioId, EnderecoPostDto enderecoPostDto)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

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
        // Cliente e Administrador podem atualizar endereço.
        [Authorize(Roles = "Cliente,Administrador")]
        [HttpPut("{usuarioId:int}/endereco/{enderecoId:int}")]
        public async Task<ActionResult> AtualizarEnderecoDoUsuario(int usuarioId, int enderecoId, EnderecoPutDto enderecoPutDto)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

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
        // Cliente e Administrador podem remover endereço.
        [Authorize(Roles = "Cliente,Administrador")]
        [HttpDelete("{usuarioId:int}/endereco/{enderecoId:int}")]
        public async Task<ActionResult> RemoverEnderecoDoUsuario(int usuarioId, int enderecoId)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

            var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            var endereco = await _enderecoService.ObterPorIdAsync(enderecoId);

            if (endereco == null || endereco.UsuarioId != usuarioId)
                return NotFound("Endereco nao encontrado.");

            await _enderecoService.RemoverAsync(enderecoId);

            return NoContent();
        }

        private bool PodeAcessarUsuario(int usuarioId)
        {
            if (User.IsInRole("Administrador"))
                return true;

            var usuarioLogadoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(usuarioLogadoId, out var id) && id == usuarioId;
        }
    }
}
