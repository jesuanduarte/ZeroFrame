using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Pedidos;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PedidosGetDto>> ObterPedidoPorId(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            return Ok(pedido);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<PedidosGetDto>>> ObterPedidosPorUsuario(int usuarioId)
        {
            var pedidos = await _pedidoService.ObterPorUsuarioAsync(usuarioId);
            return Ok(pedidos);
        }

        [HttpPost]
        public async Task<ActionResult<PedidosGetDto>> CriarPedido(PedidosPostDto pedidosPostDto)
        {
            PedidosGetDto pedidoCriado;

            try
            {
                pedidoCriado = await _pedidoService.CriarAsync(pedidosPostDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                nameof(ObterPedidoPorId),
                new { id = pedidoCriado.Id },
                pedidoCriado
            );
        }

        [HttpPost("carrinho/{carrinhoId}")]
        public async Task<ActionResult<PedidosGetDto>> CriarPedidoAPartirDoCarrinho(int carrinhoId)
        {
            PedidosGetDto pedidoCriado;

            try
            {
                pedidoCriado = await _pedidoService.CriarAPartirDoCarrinhoAsync(carrinhoId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                nameof(ObterPedidoPorId),
                new { id = pedidoCriado.Id },
                pedidoCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarPedido(int id, PedidosPutDto pedidosPutDto)
        {
            if (id != pedidosPutDto.Id)
                return BadRequest("Id da rota diferente do Id do pedido.");

            var pedido = await _pedidoService.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            await _pedidoService.AtualizarAsync(pedidosPutDto);

            return NoContent();
        }

        [HttpPatch("{id}/cancelar")]
        public async Task<ActionResult> CancelarPedido(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            await _pedidoService.CancelarAsync(id);

            return NoContent();
        }
    }
}
