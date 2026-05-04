using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Categoria;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{

    [ApiController]

    [Route("api/categorias")]
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
        public async Task<ActionResult<List<CategoriaGetDto>>> ObterTodasCategorias()
        {
            var categorias = await _categoriaService.ObterTodosAsync();
            return Ok(categorias);
        }

        // GET: api/categorias/{id}
        // Retorna uma categoria específica pelo Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaGetDto>> ObterCategoriaPorId(int id)
        {
            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound("Categoria nao encontrada.");

            return Ok(categoria);
        }

        // POST: api/categorias
        // Cria uma nova categoria
        [HttpPost]
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
        public async Task<ActionResult> AtualizarCategoria(int id, CategoriaPutDto categoriaPutDto)
        {
            if (id != categoriaPutDto.Id)
                return BadRequest("O Id da rota deve ser igual ao Id do corpo da requisicao.");

            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound("Categoria nao encontrada.");

            await _categoriaService.AtualizarAsync(categoriaPutDto);

            return NoContent();
        }

        // DELETE: api/categorias/{id}
        // Remove uma categoria
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> RemoverCategoria(int id)
        {
            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound("Categoria nao encontrada.");

            await _categoriaService.RemoverAsync(id);
            return NoContent();
        }
    }
}
