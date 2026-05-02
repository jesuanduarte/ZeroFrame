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

        // GET: api/Produto
        // Busca todos os produtos cadastrados.
        [HttpGet]
        public async Task<ActionResult<List<ProdutoGetDto>>> ObterTodosProdutos([FromQuery] ProdutoFiltroDto filtro)
        {
            var produtos = await _produtoService.ObterTodosAsync(filtro);
            return Ok(produtos);
        }

        // GET: api/Produto/{id}
        // Busca um produto específico pelo Id.
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoGetDto>> ObterProdutoPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            // Caso o produto não exista, retorna 404 Not Found.
            if (produto == null)
                return NotFound("Produto nao encontrado.");

            // Retorna o produto encontrado.
            return Ok(produto);
        }

        // POST: api/Produto
        // Cria um novo produto.
        [HttpPost]
        public async Task<ActionResult<ProdutoGetDto>> CriarProduto(ProdutoPostDto produtoPostDto)
        {
            // Envia os dados para o serviço criar o produto.
            var produtoCriado = await _produtoService.CriarAsync(produtoPostDto);

            // Retorna 201 Created informando que o produto foi criado com sucesso.
            return CreatedAtAction(
                nameof(ObterProdutoPorId),
                new { id = produtoCriado.Id },
                produtoCriado
            );
        }

        // PUT: api/Produto/{id}
        // Atualiza um produto existente.
        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarProduto(int id, ProdutoPutDto produtoPutDto)
        {
            // Verifica se o Id da rota é igual ao Id enviado no corpo da requisição.
            if (id != produtoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do produto.");

            // Busca o produto antes de atualizar, para confirmar se ele existe.
            var produto = await _produtoService.ObterPorIdAsync(id);

            // Caso o produto não exista, retorna 404 Not Found.
            if (produto == null)
                return NotFound("Produto nao encontrado.");

            // Atualiza o produto.
            await _produtoService.AtualizarAsync(produtoPutDto);

            // Retorna 204 No Content indicando que a atualização foi feita com sucesso.
            return NoContent();
        }

        // DELETE: api/Produto/{id}
        // Remove um produto existente.
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarProduto(int id)
        {
            // Busca o produto antes de remover, para confirmar se ele existe.
            var produto = await _produtoService.ObterPorIdAsync(id);

            // Caso o produto não exista, retorna 404 Not Found.
            if (produto == null)
                return NotFound("Produto nao encontrado.");

            // Remove o produto.
            await _produtoService.RemoverAsync(id);

            // Retorna 204 No Content indicando que a remoção foi feita com sucesso.
            return NoContent();
        }
    }
}

