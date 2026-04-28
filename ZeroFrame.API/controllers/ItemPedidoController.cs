using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemPedidoController : ControllerBase
    {
        private readonly IItemPedidoService _itemPedidoService;

        public ItemPedidoController(IItemPedidoService itemPedidoService)
        {
            _itemPedidoService = itemPedidoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemPedidoGetDto>>> ObterTodosItensPedido()
        {
            var itens = await _itemPedidoService.ObterTodosAsync();
            return Ok(itens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemPedidoGetDto>> ObterItemPedidoPorId(int id)
        {
            var item = await _itemPedidoService.ObterPorIdAsync(id);

            if (item == null)
                return NotFound("Item do pedido nao encontrado.");

            return Ok(item);
        }

        [HttpGet("pedido/{pedidoId}")]
        public async Task<ActionResult<List<ItemPedidoGetDto>>> ObterItensPorPedido(int pedidoId)
        {
            var itens = await _itemPedidoService.ObterPorPedidoAsync(pedidoId);
            return Ok(itens);
        }

        [HttpPost]
        public async Task<ActionResult<ItemPedidoGetDto>> CriarItemPedido(ItemPedidoPostDto itemPedidoPostDto)
        {
            var itemCriado = await _itemPedidoService.CriarAsync(itemPedidoPostDto);

            return CreatedAtAction(
                nameof(ObterItemPedidoPorId),
                new { id = itemCriado.Id },
                itemCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarItemPedido(int id, ItemPedidoPutDto itemPedidoPutDto)
        {
            if (id != itemPedidoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do item do pedido.");

            var item = await _itemPedidoService.ObterPorIdAsync(id);

            if (item == null)
                return NotFound("Item do pedido nao encontrado.");

            await _itemPedidoService.AtualizarAsync(itemPedidoPutDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarItemPedido(int id)
        {
            var item = await _itemPedidoService.ObterPorIdAsync(id);

            if (item == null)
                return NotFound("Item do pedido nao encontrado.");

            await _itemPedidoService.RemoverAsync(id);

            return NoContent();
        }
    }
}
