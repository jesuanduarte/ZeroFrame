using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.Pagamento;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IPedidoService _pedidoService;

        public PagamentoController(IPagamentoService pagamentoService, IPedidoService pedidoService)
        {
            _pagamentoService = pagamentoService;
            _pedidoService = pedidoService;
        }

        // POST: api/pedidos/{pedidoId}/pagamento
        // Registra um pagamento simples para finalizar o pedido.
        [Authorize(Roles = "Cliente,Administrador")]
        [HttpPost("pedidos/{pedidoId:int}/pagamento")]
        public async Task<ActionResult<PagamentoGetDto>> CriarPagamentoDoPedido(
            int pedidoId,
            PagamentoPedidoPostDto pagamentoPedidoPostDto)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            if (!PodeAcessarUsuario(pedido.UsuarioId))
                return Forbid();

            try
            {
                var pagamentoCriado = await _pagamentoService.CriarPagamentoDoPedidoAsync(
                    pedidoId,
                    pagamentoPedidoPostDto);

                return CreatedAtAction(
                    nameof(ObterPagamentoPorId),
                    new { id = pagamentoCriado.Id },
                    pagamentoCriado
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/pagamentos/{id}
        // Busca um pagamento pelo Id.
        [HttpGet("pagamentos/{id:int}")]
        public async Task<ActionResult<PagamentoGetDto>> ObterPagamentoPorId(int id)
        {
            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado.");

            var pedido = await _pedidoService.ObterPorIdAsync(pagamento.PedidoId);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            if (!PodeAcessarUsuario(pedido.UsuarioId))
                return Forbid();

            return Ok(pagamento);
        }

        // GET: api/pedidos/{pedidoId}/pagamento
        // Busca o pagamento associado a um pedido.
        [HttpGet("pedidos/{pedidoId:int}/pagamento")]
        public async Task<ActionResult<PagamentoGetDto>> ObterPagamentoPorPedido(int pedidoId)
        {
            var pedido = await _pedidoService.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            if (!PodeAcessarUsuario(pedido.UsuarioId))
                return Forbid();

            var pagamento = await _pagamentoService.ObterPorPedidoIdAsync(pedidoId);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado para este pedido.");

            return Ok(pagamento);
        }

        // PUT: api/pagamentos/{id}
        // Atualiza o status de um pagamento.
        [Authorize(Roles = "Administrador")]
        [HttpPut("pagamentos/{id:int}")]
        public async Task<ActionResult> AtualizarPagamento(int id, PagamentoPutDto pagamentoPutDto)
        {
            if (id != pagamentoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do pagamento.");

            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado.");

            var pedido = await _pedidoService.ObterPorIdAsync(pagamento.PedidoId);

            if (pedido == null)
                return NotFound("Pedido nao encontrado.");

            if (!PodeAcessarUsuario(pedido.UsuarioId))
                return Forbid();

            await _pagamentoService.AtualizarAsync(pagamentoPutDto);

            return NoContent();
        }

        // Método para verificar se o usuário logado pode acessar os dados do usuário informado.
        private bool PodeAcessarUsuario(int usuarioId)
        {
            if (User.IsInRole("Administrador"))
                return true;

            var usuarioLogadoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(usuarioLogadoId, out var id) && id == usuarioId;
        }
    }
}
