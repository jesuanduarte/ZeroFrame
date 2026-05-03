using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.Produto;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly IVariacaoService _variacaoService;

        public ProdutoController(IProdutoService produtoService, IVariacaoService variacaoService)
        {
            _produtoService = produtoService;
            _variacaoService = variacaoService;
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
        // Busca um produto pelo Id.
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoGetDto>> ObterProdutoPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            // Caso o produto não exista, retorna 404 Not Found.
            if (produto == null)
                return NotFound("Produto não encontrado.");

            // Retorna o produto encontrado.
            return Ok(produto);
        }

        // POST: api/Produto
        // Cria um novo produto.
        [HttpPost]
        public async Task<ActionResult<ProdutoGetDto>> CriarProduto(ProdutoPostDto produtoPostDto)
        {
            ProdutoGetDto produtoCriado;

            try
            {
                // Envia os dados para o servico criar o produto.
                produtoCriado = await _produtoService.CriarAsync(produtoPostDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }

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
            if (id != produtoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do produto.");

            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            try
            {
                // Atualiza o produto.
                await _produtoService.AtualizarAsync(produtoPutDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }

            // Retorna 204 No Content indicando que a atualização foi feita com sucesso.
            return NoContent();
        }

        // DELETE: api/Produto/{id}
        // Remove um produto existente.
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarProduto(int id)
        {
      
            var produto = await _produtoService.ObterPorIdAsync(id);

                if (produto == null)
                    return NotFound("Produto não encontrado.");

            // Remove o produto.
            await _produtoService.RemoverAsync(id);

            // Retorna 204 No Content indicando que a remoção foi feita com sucesso.
            return NoContent();
        }

        // GET: api/Produto/{produtoId}/variacoes
        // Busca as variações de um produto.
        [HttpGet("{produtoId:int}/variacoes")]
        public async Task<ActionResult<List<VariacaoGetDto>>> ObterVariacoesDoProduto(int produtoId)
        {
            try
            {
                var variacoes = await _variacaoService.ObterPorProdutoIdAsync(produtoId);
                return Ok(variacoes);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }
        }

        // GET: api/Produto/{produtoId}/variacoes/{variacaoId}
        // Busca uma variação específica de um produto.
        [HttpGet("{produtoId:int}/variacoes/{variacaoId:int}")]
        public async Task<ActionResult<VariacaoGetDto>> ObterVariacaoProdutoPorId(int produtoId, int variacaoId)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(variacaoId);

            if (variacao == null || variacao.ProdutoId != produtoId)
                return NotFound("Variação do produto não encontrada.");

            return Ok(variacao);
        }

        // POST: api/Produto/{produtoId}/variacoes
        // Cria uma variação para um produto.
        [HttpPost("{produtoId:int}/variacoes")]
        public async Task<ActionResult<VariacaoGetDto>> CriarVariacaoProduto(int produtoId, VariacaoProdutoPostDto variacaoProdutoPostDto)
        {
            VariacaoGetDto variacaoCriada;

            try
            {
                variacaoCriada = await _variacaoService.CriarAsync(new VariacaoPostDto
                {
                    Tamanho = variacaoProdutoPostDto.Tamanho,
                    Cor = variacaoProdutoPostDto.Cor,
                    Estoque = variacaoProdutoPostDto.Estoque,
                    ProdutoId = produtoId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }

            return CreatedAtAction(
                nameof(ObterVariacaoProdutoPorId),
                new { produtoId = produtoId, variacaoId = variacaoCriada.Id },
                variacaoCriada
            );
        }

        // PUT: api/Produto/{produtoId}/variacoes/{variacaoId}
        // Atualiza uma variação de um produto.
        [HttpPut("{produtoId:int}/variacoes/{variacaoId:int}")]
        public async Task<ActionResult> AtualizarVariacaoProduto(int produtoId, int variacaoId, VariacaoProdutoPutDto variacaoProdutoPutDto)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(variacaoId);

            if (variacao == null || variacao.ProdutoId != produtoId)
                return NotFound("Variacao do produto nao encontrada.");

            try
            {
                await _variacaoService.AtualizarAsync(new VariacaoPutDto
                {
                    Id = variacaoId,
                    Tamanho = variacaoProdutoPutDto.Tamanho,
                    Cor = variacaoProdutoPutDto.Cor,
                    Estoque = variacaoProdutoPutDto.Estoque,
                    ProdutoId = produtoId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }

            return NoContent();
        }

        // DELETE: api/Produto/{produtoId}/variacoes/{variacaoId}
        // Remove uma variação de um produto.
        [HttpDelete("{produtoId:int}/variacoes/{variacaoId:int}")]
        public async Task<ActionResult> RemoverVariacaoProduto(int produtoId, int variacaoId)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(variacaoId);

            if (variacao == null || variacao.ProdutoId != produtoId)
                return NotFound("Variacao do produto nao encontrada.");

            await _variacaoService.RemoverAsync(variacaoId);

            return NoContent();
        }
    }
}

