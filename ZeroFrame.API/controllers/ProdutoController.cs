using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Produto;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProdutoGetDto>>> ObterTodosProdutos()
        {
            var produtos = await _produtoService.ObterTodosAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoGetDto>> ObterProdutoPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoGetDto>> CriarProduto(ProdutoPostDto produtoPostDto)
        {
            var produtoCriado = await _produtoService.CriarAsync(produtoPostDto);

            return CreatedAtAction(
                nameof(ObterProdutoPorId),
                new { id = produtoCriado.Id },
                produtoCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarProduto(int id, ProdutoPutDto produtoPutDto)
        {
            if (id != produtoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do produto.");

            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            await _produtoService.AtualizarAsync(produtoPutDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarProduto(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            await _produtoService.RemoverAsync(id);

            return NoContent();
        }
    }
}
