using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Pedidos;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    
    [ApiController]
    [Route("api")]
    public class PedidoController : ControllerBase
    {
        // Serviço responsável pelas regras de negócio dos pedidos.
        private readonly IPedidoService _pedidoService;
        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        // GET: api/pedidos/{id}
        // Busca um pedido específico pelo seu Id.
        [HttpGet("pedidos/{id:int}")]
        public async Task<ActionResult<PedidosGetDto>> ObterPedidoPorId(int id)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            // Caso o pedido não exista, retorna 404 Not Found.
            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            // Retorna o pedido encontrado.
            return Ok(pedido);
        }

        // GET: api/usuarios/{usuarioId}/pedidos
        // Busca todos os pedidos de um usuário específico.
        [HttpGet("usuarios/{usuarioId:int}/pedidos")]
        public async Task<ActionResult<List<PedidosGetDto>>> ObterPedidosPorUsuario(int usuarioId)
        {
            var pedidos = await _pedidoService.ObterPorUsuarioAsync(usuarioId);
            return Ok(pedidos);
        }

        // POST: api/usuarios/{usuarioId}/pedidos
        // Cria um pedido a partir do carrinho ativo do usuário.
        [HttpPost("usuarios/{usuarioId:int}/pedidos")]
        public async Task<ActionResult<PedidosGetDto>> CriarPedidoAPartirDoCarrinhoAtivoDoUsuario(int usuarioId)
        {
            PedidosGetDto pedidoCriado;

            try
            {
                // Chama o serviço para transformar o carrinho ativo do usuário em um pedido.
                pedidoCriado = await _pedidoService.CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(usuarioId);
            }
            catch (InvalidOperationException ex)
            {
                // Retorna 400 Bad Request caso alguma regra de negócio seja violada.
                // Exemplo: carrinho vazio, usuário inválido ou estoque insuficiente.
                return BadRequest(ex.Message);
            }

            // Retorna 201 Created informando que o pedido foi criado com sucesso.
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
            // Verifica se o Id da rota é igual ao Id enviado no corpo da requisição.
            if (id != pedidosPutDto.Id)
                return BadRequest("Id da rota diferente do Id do pedido.");

            // Busca o pedido antes de atualizar, para confirmar se ele existe.
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            // Caso o pedido não exista, retorna 404 Not Found.
            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            // Atualiza o pedido.
            await _pedidoService.AtualizarAsync(pedidosPutDto);

            // Retorna 204 No Content indicando que a atualização foi feita com sucesso.
            return NoContent();
        }

        // PATCH: api/pedidos/{id}/cancelar
        // Cancela um pedido existente.
        [HttpPatch("pedidos/{id:int}/cancelar")]
        public async Task<ActionResult> CancelarPedido(int id)
        {
            // Busca o pedido antes de cancelar, para confirmar se ele existe.
            var pedido = await _pedidoService.ObterPorIdAsync(id);

            // Caso o pedido não exista, retorna 404 Not Found.
            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            // Chama o serviço responsável por cancelar o pedido.
            await _pedidoService.CancelarAsync(id);

            // Retorna 204 No Content indicando que o cancelamento foi feito com sucesso.
            return NoContent();
        }
    }
}

