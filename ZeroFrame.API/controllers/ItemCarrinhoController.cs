using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemCarrinhoController : ControllerBase
    {
        private readonly IItemCarrinhoService _itemCarrinhoService;

        public ItemCarrinhoController(IItemCarrinhoService itemCarrinhoService)
        {
            _itemCarrinhoService = itemCarrinhoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemCarrinhoGetDto>>> ObterTodosItensCarrinho()
        {
            var itens = await _itemCarrinhoService.ObterTodosAsync();
            return Ok(itens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemCarrinhoGetDto>> ObterItemCarrinhoPorId(int id)
        {
            var item = await _itemCarrinhoService.ObterPorIdAsync(id);

            if (item == null)
                return NotFound("Item do carrinho nao encontrado.");

            return Ok(item);
        }

        [HttpGet("carrinho/{carrinhoId}")]
        public async Task<ActionResult<List<ItemCarrinhoGetDto>>> ObterItensPorCarrinho(int carrinhoId)
        {
            var itens = await _itemCarrinhoService.ObterPorCarrinhoAsync(carrinhoId);
            return Ok(itens);
        }

        [HttpPost]
        public async Task<ActionResult<ItemCarrinhoGetDto>> CriarItemCarrinho(ItemCarrinhoPostDto itemCarrinhoPostDto)
        {
            ItemCarrinhoGetDto itemCriado;

            try
            {
                itemCriado = await _itemCarrinhoService.CriarAsync(itemCarrinhoPostDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                nameof(ObterItemCarrinhoPorId),
                new { id = itemCriado.Id },
                itemCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarItemCarrinho(int id, ItemCarrinhoPutDto itemCarrinhoPutDto)
        {
            if (id != itemCarrinhoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do item do carrinho.");

            var item = await _itemCarrinhoService.ObterPorIdAsync(id);

            if (item == null)
                return NotFound("Item do carrinho nao encontrado.");

            try
            {
                await _itemCarrinhoService.AtualizarAsync(itemCarrinhoPutDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarItemCarrinho(int id)
        {
            var item = await _itemCarrinhoService.ObterPorIdAsync(id);

            if (item == null)
                return NotFound("Item do carrinho nao encontrado.");

            await _itemCarrinhoService.RemoverAsync(id);

            return NoContent();
        }
    }
}
