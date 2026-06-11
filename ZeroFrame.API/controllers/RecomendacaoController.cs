using Microsoft.AspNetCore.Mvc;
using ZeroFrame.API.ML;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/recomendacoes")]
    public class RecomendacaoController : ControllerBase
    {
        private readonly RecomendacaoService _servico;

        public RecomendacaoController(RecomendacaoService servico)
        {
            _servico = servico;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Recomendar(int usuarioId, [FromQuery] int quantidade = 5)
        {
            var ids = await _servico.RecomendarProdutosAsync(usuarioId, quantidade);

            if (!ids.Any())
                return Ok(new { mensagem = "Ainda sem dados suficientes para recomendacao.", produtos = ids });

            return Ok(new { usuarioId, produtos = ids });
        }

        [HttpPost("treinar")]
        public async Task<IActionResult> Treinar()
        {
            await _servico.TreinarModeloAsync();
            return Ok(new { mensagem = "Modelo treinado com sucesso!" });
        }
    }
}
