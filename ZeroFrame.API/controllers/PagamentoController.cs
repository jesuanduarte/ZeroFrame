using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Pagamento;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PagamentoGetDto>> ObterPagamentoPorId(int id)
        {
            var pagamento = await _pagamentoService.ObterPorIdAsync(id);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado.");

            return Ok(pagamento);
        }

        [HttpGet("pedido/{pedidoId}")]
        public async Task<ActionResult<PagamentoGetDto>> ObterPagamentoPorPedido(int pedidoId)
        {
            var pagamento = await _pagamentoService.ObterPorPedidoIdAsync(pedidoId);

            if (pagamento == null)
                return NotFound("Pagamento nao encontrado para este pedido.");

            return Ok(pagamento);
        }

        [HttpPost]
        public async Task<ActionResult<PagamentoGetDto>> CriarPagamento(PagamentoPostDto pagamentoPostDto)
        {
            var pagamentoCriado = await _pagamentoService.CriarAsync(pagamentoPostDto);

            return CreatedAtAction(
                nameof(ObterPagamentoPorId),
                new { id = pagamentoCriado.Id },
                pagamentoCriado
            );
        }

        [HttpPut("{id}")]
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
