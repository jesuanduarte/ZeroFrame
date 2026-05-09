using Microsoft.AspNetCore.Mvc;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.DTOS.Pedidos;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    
    [ApiController]
    [Route("api")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class PedidoController : ControllerBase
    {
        // Servico responsavel pelas regras de negocio dos pedidos.
        private readonly IPedidoService _pedidoService;
        private readonly IItemPedidoService _itemPedidoService;

        public PedidoController(IPedidoService pedidoService, IItemPedidoService itemPedidoService)
        {
            _pedidoService = pedidoService;
            _itemPedidoService = itemPedidoService;
        }

        // GET: api/pedidos/{id}
        // Busca um pedido especifico pelo seu Id.
        [HttpGet("pedidos/{id:int}")]
        public async Task<ActionResult<PedidosGetDto>> ObterPedidoPorId(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            // Caso o pedido nao exista, retorna 404 Not Found.
            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            // Retorna o pedido encontrado.
            return Ok(pedido);
        }

        // GET: api/usuarios/{usuarioId}/pedidos
        // Busca todos os pedidos de um usuario especifico.
        [HttpGet("usuarios/{usuarioId:int}/pedidos")]
        public async Task<ActionResult<List<PedidosGetDto>>> ObterPedidosPorUsuario(int usuarioId)
        {
            var pedidos = await _pedidoService.ObterPorUsuarioAsync(usuarioId);
            return Ok(pedidos);
        }

        // POST: api/usuarios/{usuarioId}/pedidos
        // Cria um pedido a partir do carrinho ativo do usuario.
        [HttpPost("usuarios/{usuarioId:int}/pedidos")]
        public async Task<ActionResult<PedidosGetDto>> CriarPedidoAPartirDoCarrinhoAtivoDoUsuario(int usuarioId)
        {
            PedidosGetDto pedidoCriado;

            try
            {
                pedidoCriado = await _pedidoService.CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(usuarioId);
            }
            catch (InvalidOperationException ex)
            {
                // Exemplo: carrinho vazio, usuario invalido ou estoque insuficiente.
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                nameof(ObterPedidoPorId),
                new { id = pedidoCriado.Id },
                pedidoCriado
            );
        }

        // PUT: api/pedidos/{id}
        // Atualiza os dados de um pedido existente.
        [HttpPut("pedidos/{id:int}")]
        public async Task<ActionResult> AtualizarPedido(int id, PedidosPutDto pedidosPutDto)
        {
            // Verifica se o Id da rota e igual ao Id enviado no corpo da requisicao.
            if (id != pedidosPutDto.Id)
                return BadRequest("Id da rota diferente do Id do pedido.");

            var pedido = await _pedidoService.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            await _pedidoService.AtualizarAsync(pedidosPutDto);

            // Retorna 204 No Content indicando que a atualizacao foi feita com sucesso.
            return NoContent();
        }

        // PATCH: api/pedidos/{id}/cancelar
        // Cancela um pedido existente.
        [HttpPatch("pedidos/{id:int}/cancelar")]
        public async Task<ActionResult> CancelarPedido(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            await _pedidoService.CancelarAsync(id);

            // Retorna 204 No Content indicando que o cancelamento foi feito com sucesso.
            return NoContent();
        }
        // GET: api/pedidos/{pedidoId}/itens
        // Busca os itens de um pedido.
        [HttpGet("pedidos/{pedidoId:int}/itens")]
        public async Task<ActionResult<List<ItemPedidoGetDto>>> ObterItensDoPedido(int pedidoId)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            var itens = await _itemPedidoService.ObterPorPedidoAsync(pedidoId);

            return Ok(itens);
        }

        // GET: api/pedidos/{pedidoId}/itens/{itemId}
        // Busca um item de um pedido.
        [HttpGet("pedidos/{pedidoId:int}/itens/{itemId:int}")]
        public async Task<ActionResult<ItemPedidoGetDto>> ObterItemDoPedidoPorId(int pedidoId, int itemId)
        {
            var item = await _itemPedidoService.ObterPorIdAsync(itemId);

            if (item == null || item.PedidoId != pedidoId)
                return NotFound("Item do pedido nao encontrado.");

            return Ok(item);
        }

        // POST: api/pedidos/{pedidoId}/itens
        // Cria um item para um pedido.
        [HttpPost("pedidos/{pedidoId:int}/itens")]
        public async Task<ActionResult<ItemPedidoGetDto>> CriarItemDoPedido(int pedidoId, PedidoItemPostDto pedidoItemPostDto)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            ItemPedidoGetDto itemCriado;

            try
            {
                itemCriado = await _itemPedidoService.CriarAsync(new ItemPedidoPostDto
                {
                    PedidoId = pedidoId,
                    VariacaoProdutoId = pedidoItemPostDto.VariacaoProdutoId,
                    Quantidade = pedidoItemPostDto.Quantidade
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction(
                nameof(ObterItemDoPedidoPorId),
                new { pedidoId = pedidoId, itemId = itemCriado.Id },
                itemCriado
            );
        }

        // PUT: api/pedidos/{pedidoId}/itens/{itemId}
        // Atualiza um item de um pedido.
        [HttpPut("pedidos/{pedidoId:int}/itens/{itemId:int}")]
        public async Task<ActionResult> AtualizarItemDoPedido(int pedidoId, int itemId, PedidoItemPutDto pedidoItemPutDto)
        {
            var item = await _itemPedidoService.ObterPorIdAsync(itemId);

            if (item == null || item.PedidoId != pedidoId)
                return NotFound("Item do pedido nao encontrado.");

            try
            {
                await _itemPedidoService.AtualizarAsync(new ItemPedidoPutDto
                {
                    Id = itemId,
                    PedidoId = pedidoId,
                    VariacaoProdutoId = pedidoItemPutDto.VariacaoProdutoId,
                    Quantidade = pedidoItemPutDto.Quantidade
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        // DELETE: api/pedidos/{pedidoId}/itens/{itemId}
        // Remove um item de um pedido.
        [HttpDelete("pedidos/{pedidoId:int}/itens/{itemId:int}")]
        public async Task<ActionResult> RemoverItemDoPedido(int pedidoId, int itemId)
        {
            var item = await _itemPedidoService.ObterPorIdAsync(itemId);

            if (item == null || item.PedidoId != pedidoId)
                return NotFound("Item do pedido nao encontrado.");

            try
            {
                await _itemPedidoService.RemoverAsync(itemId);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }
    }
}