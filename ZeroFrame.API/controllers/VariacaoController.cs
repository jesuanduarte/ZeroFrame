using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VariacaoController : ControllerBase
    {
        private readonly IVariacaoService _variacaoService;

        public VariacaoController(IVariacaoService variacaoService)
        {
            _variacaoService = variacaoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VariacaoGetDto>>> ObterTodasVariacoes()
        {
            var variacoes = await _variacaoService.ObterTodosAsync();
            return Ok(variacoes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VariacaoGetDto>> ObterVariacaoPorId(int id)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(id);

            if (variacao == null)
                return NotFound("Variacao nao encontrada.");

            return Ok(variacao);
        }

        [HttpPost]
        public async Task<ActionResult<VariacaoGetDto>> CriarVariacao(VariacaoPostDto variacaoPostDto)
        {
            var variacaoCriada = await _variacaoService.CriarAsync(variacaoPostDto);

            return CreatedAtAction(
                nameof(ObterVariacaoPorId),
                new { id = variacaoCriada.Id },
                variacaoCriada
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarVariacao(int id, VariacaoPutDto variacaoPutDto)
        {
            if (id != variacaoPutDto.Id)
                return BadRequest("Id da rota diferente do Id da variacao.");

            var variacao = await _variacaoService.ObterPorIdAsync(id);

            if (variacao == null)
                return NotFound("Variacao nao encontrada.");

            await _variacaoService.AtualizarAsync(variacaoPutDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarVariacao(int id)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(id);

            if (variacao == null)
                return NotFound("Variacao nao encontrada.");

            await _variacaoService.RemoverAsync(id);

            return NoContent();
        }
    }
}
