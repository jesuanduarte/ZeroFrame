using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/suporte")]
    public class SuporteController : ControllerBase
    {
        [HttpPost]
        public ActionResult EnviarMensagem(SuportePostDto suportePostDto)
        {
            return Ok(new
            {
                mensagem = "Mensagem de suporte recebida com sucesso.",
                protocolo = $"ZF-{DateTime.UtcNow:yyyyMMddHHmmss}"
            });
        }
    }
}