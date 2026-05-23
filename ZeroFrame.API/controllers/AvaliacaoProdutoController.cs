using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS.AvaliacaoProduto;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class AvaliacaoProdutoController : ControllerBase
    {
        private readonly IAvaliacaoProdutoService _avaliacaoProdutoService;

        public AvaliacaoProdutoController(IAvaliacaoProdutoService avaliacaoProdutoService)
        {
            _avaliacaoProdutoService = avaliacaoProdutoService;
        }

        [Authorize]
        [HttpPost("produtos/{produtoId:int}/avaliacoes")]
        // Cria uma nova avaliação para um produto.
        public async Task<ActionResult<AvaliacaoProdutoGetDto>> CriarAvaliacao(
        int produtoId,
        AvaliacaoProdutoPostDto avaliacaoProdutoPostDto)
        {
            try
            {
                // Chama o serviço para criar a avaliação usando o produto, dados enviados e usuário logado.
                var avaliacao = await _avaliacaoProdutoService.CriarAsync(
                    produtoId,
                    avaliacaoProdutoPostDto,
                    ObterUsuarioLogadoId());

                // Retorna a avaliação criada com status 201.
                return CreatedAtAction(
                    nameof(ListarAvaliacoesDoProduto),
                    new { produtoId = produtoId },
                    avaliacao);
            }
            catch (UnauthorizedAccessException)
            {
                // Retorna acesso negado quando o usuário não tem permissão.
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                // Retorna 404 quando algum registro não é encontrado.
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                // Retorna 400 quando os dados enviados são inválidos.
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Retorna 400 quando alguma regra de negócio é violada.
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("produtos/{produtoId:int}/avaliacoes/minha-avaliacao")]
        // Atualiza a avaliação do usuário logado para um produto.
        public async Task<ActionResult<AvaliacaoProdutoGetDto>> AtualizarMinhaAvaliacao(
            int produtoId,
            AvaliacaoProdutoPutDto avaliacaoProdutoPutDto)
        {
            try
            {
                // Chama o serviço para atualizar a avaliação do usuário logado.
                var avaliacao = await _avaliacaoProdutoService.AtualizarMinhaAvaliacaoAsync(
                    produtoId,
                    avaliacaoProdutoPutDto,
                    ObterUsuarioLogadoId());

                // Retorna a avaliação atualizada.
                return Ok(avaliacao);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("produtos/{produtoId:int}/avaliacoes")]
        // Lista as avaliações ativas de um produto com paginação.
        public async Task<ActionResult<PagedResponse<AvaliacaoProdutoGetDto>>> ListarAvaliacoesDoProduto(
            int produtoId,
            [FromQuery] PaginationParams paginationParams)
        {
            try
            {
                // Busca as avaliações ativas do produto de forma paginada.
                var avaliacoes = await _avaliacaoProdutoService.ListarAtivasPorProdutoPaginadoAsync(
                    produtoId,
                    paginationParams);

                // Retorna a lista de avaliações.
                return Ok(avaliacoes);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("produtos/{produtoId:int}/avaliacoes/resumo")]
        // Retorna o resumo das avaliações de um produto.
        public async Task<ActionResult<AvaliacaoResumoDto>> ObterResumoAvaliacoesDoProduto(int produtoId)
        {
            try
            {
                // Busca o resumo das avaliações, como média e quantidade.
                var resumo = await _avaliacaoProdutoService.ObterResumoAsync(produtoId);

                // Retorna o resumo encontrado.
                return Ok(resumo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("admin/avaliacoes/{avaliacaoId:int}")]
        // Apaga uma avaliação pelo ID. Ação permitida apenas para administrador.
        public async Task<ActionResult> ApagarAvaliacao(int avaliacaoId)
        {
            try
            {
                // Chama o serviço para apagar a avaliação.
                await _avaliacaoProdutoService.ApagarAsync(avaliacaoId, User.IsInRole("Administrador"));

                // Retorna 204 indicando que a ação foi concluída sem conteúdo.
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("admin/avaliacoes/{avaliacaoId:int}/desativar")]
        // Desativa uma avaliação sem remover definitivamente do banco.
        public async Task<ActionResult> DesativarAvaliacao(int avaliacaoId)
        {
            try
            {
                // Chama o serviço para desativar a avaliação.
                await _avaliacaoProdutoService.DesativarAsync(avaliacaoId, User.IsInRole("Administrador"));

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPatch("admin/avaliacoes/{avaliacaoId:int}/comentario")]
        // Permite ao administrador moderar o comentário de uma avaliação.
        public async Task<ActionResult<AvaliacaoProdutoGetDto>> ModerarComentario(
            int avaliacaoId,
            AvaliacaoProdutoComentarioModeracaoDto avaliacaoProdutoComentarioModeracaoDto)
        {
            try
            {
                // Chama o serviço para atualizar/moderar o comentário.
                var avaliacao = await _avaliacaoProdutoService.ModerarComentarioAsync(
                    avaliacaoId,
                    avaliacaoProdutoComentarioModeracaoDto,
                    User.IsInRole("Administrador"));

                return Ok(avaliacao);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Obtém o ID do usuário logado a partir do token JWT.
        private int? ObterUsuarioLogadoId()
        {
            // Busca o identificador do usuário dentro das claims do token.
            var usuarioLogadoId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Converte o ID para inteiro. Se não conseguir, retorna null.
            return int.TryParse(usuarioLogadoId, out var id) ? id : null;
        }
    }
}
