using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoService _carrinhoService;

        public CarrinhoController(ICarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CarrinhoGetDto>>> ObterTodosCarrinhos()
        {
            var carrinhos = await _carrinhoService.ObterTodosAsync();
            return Ok(carrinhos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarrinhoGetDto>> ObterCarrinhoPorId(int id)
        {
            var carrinho = await _carrinhoService.ObterPorIdAsync(id);

            if (carrinho == null)
                return NotFound("Carrinho nao encontrado.");

            return Ok(carrinho);
        }

        [HttpPost("usuario/{usuarioId}/ativo")]
        public async Task<ActionResult<CarrinhoGetDto>> ObterOuCriarCarrinhoAtivoPorUsuario(int usuarioId)
        {
            var carrinho = await _carrinhoService.ObterOuCriarAtivoPorUsuarioAsync(usuarioId);
            return Ok(carrinho);
        }

        [HttpPost]
        public async Task<ActionResult<CarrinhoGetDto>> CriarCarrinho(CarrinhoPostDto carrinhoPostDto)
        {
            var carrinhoCriado = await _carrinhoService.CriarAsync(carrinhoPostDto);

            return CreatedAtAction(
                nameof(ObterCarrinhoPorId),
                new { id = carrinhoCriado.Id },
                carrinhoCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarCarrinho(int id, CarrinhoPutDto carrinhoPutDto)
        {
            if (id != carrinhoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do carrinho.");

            var carrinho = await _carrinhoService.ObterPorIdAsync(id);

            if (carrinho == null)
                return NotFound("Carrinho nao encontrado.");

            await _carrinhoService.AtualizarAsync(carrinhoPutDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarCarrinho(int id)
        {
            var carrinho = await _carrinhoService.ObterPorIdAsync(id);

            if (carrinho == null)
                return NotFound("Carrinho nao encontrado.");

            await _carrinhoService.RemoverAsync(id);

            return NoContent();
        }
    }
}
