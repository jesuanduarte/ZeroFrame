using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.Common;
using ZeroFrame.Application.DTOS.Produto;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Route("api/Produto")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly IVariacaoService _variacaoService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const long TamanhoMaximoImagemEmBytes = 5 * 1024 * 1024;
        private static readonly string[] ExtensoesPermitidas = [".jpg", ".jpeg", ".png", ".webp"];
        private static readonly string[] ContentTypesPermitidos = ["image/jpeg", "image/png", "image/webp"];

        public ProdutoController(
            IProdutoService produtoService,
            IVariacaoService variacaoService,
            IWebHostEnvironment webHostEnvironment)
        {
            _produtoService = produtoService;
            _variacaoService = variacaoService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Produto
        // Busca todos os produtos cadastrados.
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ProdutoGetDto>>> ObterTodosProdutos(
            [FromQuery] ProdutoFiltroDto filtro,
            [FromQuery] PaginationParams paginationParams)
        {
            var produtos = await _produtoService.ObterTodosPaginadoAsync(filtro, paginationParams);
            return Ok(produtos);
        }

        // GET: api/produtos/admin/todos
        // Lista produtos ativos e inativos para a area administrativa.
        [Authorize(Roles = "Administrador")]
        [HttpGet("admin/todos")]
        public async Task<ActionResult<PagedResponse<ProdutoGetDto>>> ObterTodosProdutosAdmin(
            [FromQuery] PaginationParams paginationParams)
        {
            var produtos = await _produtoService.ObterTodosAdminPaginadoAsync(paginationParams);
            return Ok(produtos);
        }

        // GET: api/Produto/{id}
        // Busca um produto pelo Id.
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoGetDto>> ObterProdutoPorId(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            return Ok(produto);
        }

        // POST: api/Produto
        // Cria um novo produto via multipart/form-data, com imagem opcional.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProdutoGetDto>> CriarProduto([FromForm] ProdutoPostDto produtoPostDto)
        {
            ProdutoGetDto produtoCriado;
            string? imagemSalva = null;

            try
            {
                // Salva a imagem no wwwroot e envia para a aplicacao apenas a URL publica.
                imagemSalva = await SalvarImagemProdutoAsync(produtoPostDto.ImagemArquivo);
                if (imagemSalva != null)
                    produtoPostDto.ImagemUrl = imagemSalva;

                produtoCriado = await _produtoService.CriarAsync(produtoPostDto);
            }
            catch (InvalidOperationException ex)
            {
                ApagarImagemSeExistir(imagemSalva);
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }

            return CreatedAtAction(
                nameof(ObterProdutoPorId),
                new { id = produtoCriado.Id },
                produtoCriado
            );
        }

        // PUT: api/Produto/{id}
        // Atualiza um produto existente e substitui a imagem apenas quando um novo arquivo for enviado.
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProdutoGetDto>> AtualizarProduto(int id, [FromForm] ProdutoPutDto produtoPutDto)
        {
            if (id != produtoPutDto.Id)
                return BadRequest("Id da rota diferente do Id do produto.");

            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            string? imagemSalva = null;
            var imagemAntiga = produto.ImagemUrl;

            try
            {
                // Se uma nova imagem foi enviada, ela substitui a URL antiga; caso contrario, o service preserva a atual.
                imagemSalva = await SalvarImagemProdutoAsync(produtoPutDto.ImagemArquivo);
                if (imagemSalva != null)
                    produtoPutDto.ImagemUrl = imagemSalva;

                await _produtoService.AtualizarAsync(produtoPutDto);
            }
            catch (InvalidOperationException ex)
            {
                ApagarImagemSeExistir(imagemSalva);
                return BadRequest(new { status = 400, mensagem = ex.Message });
            }

            if (imagemSalva != null && await ImagemNaoEstaEmUsoAsync(imagemAntiga, id))
                ApagarImagemSeExistir(imagemAntiga);

            var produtoAtualizado = await _produtoService.ObterPorIdAsync(id);
            return Ok(produtoAtualizado);
        }

        // DELETE: api/Produto/{id}
        // Remove um produto existente.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletarProduto(int id)
        {
            var produto = await _produtoService.ObterPorIdAsync(id);

            if (produto == null)
                return NotFound("Produto nao encontrado.");

            // Soft delete: preserva historico financeiro e pedidos antigos.
            await _produtoService.RemoverAsync(id);

            return NoContent();
        }

        // GET: api/Produto/{produtoId}/variacoes
        // Busca as variacoes de um produto.
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
        // Busca uma variacao especifica de um produto.
        [HttpGet("{produtoId:int}/variacoes/{variacaoId:int}")]
        public async Task<ActionResult<VariacaoGetDto>> ObterVariacaoProdutoPorId(int produtoId, int variacaoId)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(variacaoId);

            if (variacao == null || variacao.ProdutoId != produtoId)
                return NotFound("Variacao do produto nao encontrada.");

            return Ok(variacao);
        }

        // POST: api/Produto/{produtoId}/variacoes
        // Cria uma variacao para um produto.
        [Authorize(Roles = "Administrador")]
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
        // Atualiza uma variacao de um produto.
        [Authorize(Roles = "Administrador")]
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
        // Remove uma variacao de um produto.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{produtoId:int}/variacoes/{variacaoId:int}")]
        public async Task<ActionResult> RemoverVariacaoProduto(int produtoId, int variacaoId)
        {
            var variacao = await _variacaoService.ObterPorIdAsync(variacaoId);

            if (variacao == null || variacao.ProdutoId != produtoId)
                return NotFound("Variacao do produto nao encontrada.");

            await _variacaoService.RemoverAsync(variacaoId);

            return NoContent();
        }

        private async Task<string?> SalvarImagemProdutoAsync(IFormFile? imagemArquivo)
        {
            if (imagemArquivo == null || imagemArquivo.Length == 0)
                return null;

            if (imagemArquivo.Length > TamanhoMaximoImagemEmBytes)
                throw new InvalidOperationException("A imagem deve ter no maximo 5MB.");

            var extensao = Path.GetExtension(imagemArquivo.FileName).ToLowerInvariant();
            if (!ExtensoesPermitidas.Contains(extensao))
                throw new InvalidOperationException("A imagem deve estar nos formatos .jpg, .jpeg, .png ou .webp.");

            if (!ContentTypesPermitidos.Contains(imagemArquivo.ContentType.ToLowerInvariant()))
                throw new InvalidOperationException("O arquivo enviado precisa ser uma imagem valida.");

            if (!await ArquivoPossuiAssinaturaDeImagemAsync(imagemArquivo, extensao))
                throw new InvalidOperationException("O conteudo do arquivo nao corresponde a uma imagem valida.");

            var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
            var pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath ?? "wwwroot", "uploads", "produtos");
            Directory.CreateDirectory(pastaUploads);

            var caminhoFisico = Path.Combine(pastaUploads, nomeArquivo);

            await using (var stream = new FileStream(caminhoFisico, FileMode.CreateNew))
            {
                await imagemArquivo.CopyToAsync(stream);
            }

            // Caminho publico salvo no banco. Nunca salvar caminho absoluto do Windows.
            return $"/uploads/produtos/{nomeArquivo}";
        }

        private static async Task<bool> ArquivoPossuiAssinaturaDeImagemAsync(IFormFile imagemArquivo, string extensao)
        {
            var buffer = new byte[12];

            await using var stream = imagemArquivo.OpenReadStream();
            var bytesLidos = await stream.ReadAsync(buffer);

            return extensao switch
            {
                ".jpg" or ".jpeg" => bytesLidos >= 3
                    && buffer[0] == 0xFF
                    && buffer[1] == 0xD8
                    && buffer[2] == 0xFF,
                ".png" => bytesLidos >= 8
                    && buffer[0] == 0x89
                    && buffer[1] == 0x50
                    && buffer[2] == 0x4E
                    && buffer[3] == 0x47
                    && buffer[4] == 0x0D
                    && buffer[5] == 0x0A
                    && buffer[6] == 0x1A
                    && buffer[7] == 0x0A,
                ".webp" => bytesLidos >= 12
                    && buffer[0] == 0x52
                    && buffer[1] == 0x49
                    && buffer[2] == 0x46
                    && buffer[3] == 0x46
                    && buffer[8] == 0x57
                    && buffer[9] == 0x45
                    && buffer[10] == 0x42
                    && buffer[11] == 0x50,
                _ => false
            };
        }

        private async Task<bool> ImagemNaoEstaEmUsoAsync(string? imagemUrl, int produtoIdIgnorado)
        {
            if (!ImagemEhUploadDeProduto(imagemUrl))
                return false;

            var produtos = await _produtoService.ObterTodosAsync();
            return !produtos.Any(produto => produto.Id != produtoIdIgnorado && produto.ImagemUrl == imagemUrl);
        }

        private void ApagarImagemSeExistir(string? imagemUrl)
        {
            if (!ImagemEhUploadDeProduto(imagemUrl))
                return;

            imagemUrl ??= string.Empty;
            var caminhoRelativo = imagemUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var caminhoFisico = Path.Combine(_webHostEnvironment.WebRootPath ?? "wwwroot", caminhoRelativo);

            if (System.IO.File.Exists(caminhoFisico))
                System.IO.File.Delete(caminhoFisico);
        }

        private static bool ImagemEhUploadDeProduto(string? imagemUrl)
        {
            return !string.IsNullOrWhiteSpace(imagemUrl)
                && imagemUrl.StartsWith("/uploads/produtos/", StringComparison.OrdinalIgnoreCase);
        }
    }
}
