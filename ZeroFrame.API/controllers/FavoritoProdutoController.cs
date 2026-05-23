using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.FavoritoProduto;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Cliente,Administrador")]
    [Route("api/usuarios/{usuarioId:int}/favoritos")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class FavoritoProdutoController : ControllerBase
    {
        private readonly IFavoritoProdutoService _favoritoProdutoService;

        public FavoritoProdutoController(IFavoritoProdutoService favoritoProdutoService)
        {
            _favoritoProdutoService = favoritoProdutoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<FavoritoProdutoGetDto>>> ObterFavoritosDoUsuario(int usuarioId)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

            var favoritos = await _favoritoProdutoService.ObterPorUsuarioAsync(usuarioId);
            return Ok(favoritos);
        }

        [HttpPost("{produtoId:int}")]
        public async Task<ActionResult<FavoritoProdutoGetDto>> AdicionarFavorito(int usuarioId, int produtoId)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

            try
            {
                var favorito = await _favoritoProdutoService.AdicionarAsync(usuarioId, produtoId);
                return Ok(favorito);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{produtoId:int}")]
        public async Task<ActionResult> RemoverFavorito(int usuarioId, int produtoId)
        {
            if (!PodeAcessarUsuario(usuarioId))
                return Forbid();

            try
            {
                await _favoritoProdutoService.RemoverAsync(usuarioId, produtoId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        private bool PodeAcessarUsuario(int usuarioId)
        {
            if (User.IsInRole("Administrador"))
                return true;

            var usuarioLogadoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(usuarioLogadoId, out var id) && id == usuarioId;
        }
    }
}
