using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Endereco;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EnderecoGetDto>>> ObterTodosEnderecos()
        {
            var enderecos = await _enderecoService.ObterTodosAsync();
            return Ok(enderecos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnderecoGetDto>> ObterEnderecoPorId(int id)
        {
            var endereco = await _enderecoService.ObterPorIdAsync(id);

            if (endereco == null)
                return NotFound("Endereco nao encontrado.");

            return Ok(endereco);
        }

        [HttpPost]
        public async Task<ActionResult<EnderecoGetDto>> CriarEndereco(EnderecoPostDto enderecoPostDto)
        {
            var enderecoCriado = await _enderecoService.CriarAsync(enderecoPostDto);

            return CreatedAtAction(
                nameof(ObterEnderecoPorId),
                new { id = enderecoCriado.Id },
                enderecoCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarEndereco(int id, EnderecoPutDto enderecoPutDto)
        {
            if (id != enderecoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do endereco.");

            var endereco = await _enderecoService.ObterPorIdAsync(id);

            if (endereco == null)
                return NotFound("Endereco nao encontrado.");

            await _enderecoService.AtualizarAsync(enderecoPutDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarEndereco(int id)
        {
            var endereco = await _enderecoService.ObterPorIdAsync(id);

            if (endereco == null)
                return NotFound("Endereco nao encontrado.");

            await _enderecoService.RemoverAsync(id);

            return NoContent();
        }
    }
}
