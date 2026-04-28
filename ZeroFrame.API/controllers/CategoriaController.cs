using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Categoria;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaGetDto>>> ObterTodasCategorias()
        {
            var categorias = await _categoriaService.ObterTodosAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaGetDto>> ObterCategoriaPorId(int id)
        {
            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound("Categoria nao encontrada.");

            return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaGetDto>> CriarCategoria(CategoriaPostDto categoriaPostDto)
        {
            var categoriaCriada = await _categoriaService.CriarAsync(categoriaPostDto);

            return CreatedAtAction(
                nameof(ObterCategoriaPorId),
                new { id = categoriaCriada.Id },
                categoriaCriada
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarCategoria(int id, CategoriaPutDto categoriaPutDto)
        {
            if (id != categoriaPutDto.Id)
                return BadRequest("Id da rota diferente do Id da categoria.");

            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound("Categoria nao encontrada.");

            await _categoriaService.AtualizarAsync(categoriaPutDto);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarCategoria(int id)
        {
            var categoria = await _categoriaService.ObterPorIdAsync(id);

            if (categoria == null)
                return NotFound("Categoria nao encontrada.");

            await _categoriaService.RemoverAsync(id);

            return NoContent();
        }
    }
}
