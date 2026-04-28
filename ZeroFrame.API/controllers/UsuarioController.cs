using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Usuario;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioGetDto>> CriarUsuario(UsuarioPostDto usuarioPostDto)
        {
            var usuarioCriado = await _usuarioService.CriarAsync(usuarioPostDto);

            return CreatedAtAction(
                nameof(ObterUsuarioPorId),
                new { id = usuarioCriado.Id },
                usuarioCriado
            );
        }

        [HttpPut]
        public async Task<ActionResult> AtualizarUsuario(UsuarioPutDto usuarioPutDto)
        {
            await _usuarioService.AtualizarAsync(usuarioPutDto);
            return Ok("Usuário atualizado com sucesso!");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioGetDto>> ObterUsuarioPorId(int id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoverUsuario(int id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            await _usuarioService.RemoverAsync(id);

            return Ok("Usuário removido com sucesso!");
        }
    }
}

