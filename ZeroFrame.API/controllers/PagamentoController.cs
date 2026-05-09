using Microsoft.AspNetCore.Mvc;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.Pagamento;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        // POST: api/pedidos/{pedidoId}/pagamento
        // Registra um pagamento simples para finalizar o pedido.
        [HttpPost("pedidos/{pedidoId:int}/pagamento")]
        public async Task<ActionResult<PagamentoGetDto>> CriarPagamentoDoPedido(
            int pedidoId,
            PagamentoPedidoPostDto pagamentoPedidoPostDto)
        {
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

            return Ok(pagamento);
        }

        // GET: api/pedidos/{pedidoId}/pagamento
        // Busca o pagamento associado a um pedido.
        [HttpGet("pedidos/{pedidoId:int}/pagamento")]
        public async Task<ActionResult<PagamentoGetDto>> ObterPagamentoPorPedido(int pedidoId)
        {
            var pagamento = await _pagamentoService.ObterPorPedidoIdAsync(pedidoId);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado para este pedido.");

            return Ok(pagamento);
        }

        // PUT: api/pagamentos/{id}
        // Atualiza o status de um pagamento.
        [HttpPut("pagamentos/{id:int}")]
        public async Task<ActionResult> AtualizarPagamento(int id, PagamentoPutDto pagamentoPutDto)
        {
            if (id != pagamentoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do pagamento.");

            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado.");

            await _pagamentoService.AtualizarAsync(pagamentoPutDto);

            return NoContent();
        }
    }
}