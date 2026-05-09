using Microsoft.AspNetCore.Mvc;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.Categoria;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{

    [ApiController]

    [Route("api/categorias")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        // GET: api/categorias
        // Retorna todas as categorias cadastradas
        [HttpGet]
        [ProducesResponseType(typeof(List<CategoriaGetDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoriaGetDto>>> ObterTodasCategorias()
        {
            var categorias = await _categoriaService.ObterTodosAsync();
            return Ok(categorias);
        }

        // GET: api/categorias/{id}
        // Retorna uma categoria especÃ­fica pelo Id
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoriaGetDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CategoriaGetDto>> ObterCategoriaPorId(int id)
        {
            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound(new ApiNotFound("Categoria nao encontrada."));

            return Ok(categoria);
        }

        // POST: api/categorias
        // Cria uma nova categoria
        [HttpPost]
        [ProducesResponseType(typeof(CategoriaGetDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<CategoriaGetDto>> CriarCategoria(CategoriaPostDto categoriaPostDto)
        {
            var categoriaCriada = await _categoriaService.CriarAsync(categoriaPostDto);

            return CreatedAtAction(
                nameof(ObterCategoriaPorId),
                new { id = categoriaCriada.Id },
                categoriaCriada);
        }

        // PUT: api/categorias/{id}
        // Atualiza uma categoria existente
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AtualizarCategoria(int id, CategoriaPutDto categoriaPutDto)
        {
            if (id != categoriaPutDto.Id)
                return BadRequest(new ApiBadRequest("O Id da rota deve ser igual ao Id do corpo da requisicao."));

            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound(new ApiNotFound("Categoria nao encontrada."));

            await _categoriaService.AtualizarAsync(categoriaPutDto);

            return NoContent();
        }

        // DELETE: api/categorias/{id}
        // Remove uma categoria
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RemoverCategoria(int id)
        {
            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound(new ApiNotFound("Categoria nao encontrada."));

            await _categoriaService.RemoverAsync(id);
            return NoContent();
        }
    }
}
