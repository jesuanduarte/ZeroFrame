using Microsoft.AspNetCore.Mvc;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.DTOS;
using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
   
    [ApiController]
    [Route("api/usuarios/{usuarioId:int}/carrinho")]
    [ProducesResponseType(typeof(ApiBadRequest), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiNotFound), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiException), StatusCodes.Status500InternalServerError)]
    public class CarrinhoController : ControllerBase
    {
        // Serviço responsável pelas regras do carrinho.
        private readonly ICarrinhoService _carrinhoService;
        private readonly IItemCarrinhoService _itemCarrinhoService;

        public CarrinhoController(
            ICarrinhoService carrinhoService,
            IItemCarrinhoService itemCarrinhoService)
        {
            _carrinhoService = carrinhoService;
            _itemCarrinhoService = itemCarrinhoService;
        }

        // GET: api/usuarios/{usuarioId}/carrinho
        // Busca o carrinho ativo do usuÃ¡rio.
        // Caso o usuÃ¡rio ainda nÃ£o tenha um carrinho ativo, o serviÃ§o pode criar um automaticamente.
        [HttpGet]
        public async Task<ActionResult<CarrinhoGetDto>> ObterCarrinhoAtivoDoUsuario(int usuarioId)
        {
            var carrinho = await _carrinhoService.ObterOuCriarAtivoPorUsuarioAsync(usuarioId);
            return Ok(carrinho);
        }

        // GET: api/usuarios/{usuarioId}/carrinho/itens
        // Busca todos os itens existentes dentro do carrinho ativo do usuÃ¡rio.
        [HttpGet("itens")]
        public async Task<ActionResult<List<ItemCarrinhoGetDto>>> ObterItensDoCarrinho(int usuarioId)
        {
            var carrinho = await _carrinhoService.ObterOuCriarAtivoPorUsuarioAsync(usuarioId);
            var itens = await _itemCarrinhoService.ObterPorCarrinhoAsync(carrinho.Id);
            return Ok(itens);
        }

        // POST: api/usuarios/{usuarioId}/carrinho/itens
        // Adiciona um novo item ao carrinho ativo do usuÃ¡rio.
        [HttpPost("itens")]
        public async Task<ActionResult<ItemCarrinhoGetDto>> AdicionarItemAoCarrinho(
            int usuarioId,
            ItemCarrinhoUsuarioPostDto itemCarrinhoUsuarioPostDto)
        {
            // Busca ou cria o carrinho ativo do usuÃ¡rio antes de adicionar o item.
            var carrinho = await _carrinhoService.ObterOuCriarAtivoPorUsuarioAsync(usuarioId);

            // Monta o DTO usado pelo serviÃ§o de item carrinho.
            // O usuÃ¡rio nÃ£o precisa informar o CarrinhoId diretamente,
            // pois ele Ã© obtido a partir do carrinho ativo.
            var itemCarrinhoPostDto = new ItemCarrinhoPostDto
            {
                CarrinhoId = carrinho.Id,
                VariacaoProdutoId = itemCarrinhoUsuarioPostDto.VariacaoProdutoId,
                Quantidade = itemCarrinhoUsuarioPostDto.Quantidade
            };

            try
            {
                // Cria o item no carrinho.
                var itemCriado = await _itemCarrinhoService.CriarAsync(itemCarrinhoPostDto);

                // Retorna status 201 Created informando que o item foi criado com sucesso.
                return CreatedAtAction(
                    nameof(ObterItensDoCarrinho),
                    new { usuarioId },
                    itemCriado
                );
            }
            catch (InvalidOperationException ex)
            {
                // Retorna erro 400 caso alguma regra de negÃ³cio seja violada.
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/usuarios/{usuarioId}/carrinho/itens/{itemId}
        // Atualiza um item especÃ­fico do carrinho do usuÃ¡rio.
        [HttpPut("itens/{itemId:int}")]
        public async Task<ActionResult> AtualizarItemDoCarrinho(
            int usuarioId,
            int itemId,
            ItemCarrinhoPutDto itemCarrinhoPutDto)
        {
            // Busca o carrinho ativo do usuÃ¡rio.
            var carrinho = await _carrinhoService.ObterOuCriarAtivoPorUsuarioAsync(usuarioId);

            // Busca o item informado pelo id.
            var item = await _itemCarrinhoService.ObterPorIdAsync(itemId);

            // Verifica se o item existe e se pertence ao carrinho do usuÃ¡rio.
            if (item == null || item.CarrinhoId != carrinho.Id)
                return NotFound("Item do carrinho nao encontrado.");

            // Garante que o DTO serÃ¡ atualizado com o id vindo da rota.
            itemCarrinhoPutDto.Id = itemId;

            try
            {
                // Atualiza o item do carrinho.
                await _itemCarrinhoService.AtualizarAsync(itemCarrinhoPutDto);
            }
            catch (InvalidOperationException ex)
            {
                // Retorna erro 400 caso alguma regra de negÃ³cio seja violada.
                return BadRequest(ex.Message);
            }

            // Retorna 204 No Content indicando que a atualizaÃ§Ã£o foi feita com sucesso.
            return NoContent();
        }

        // DELETE: api/usuarios/{usuarioId}/carrinho/itens/{itemId}
        // Remove um item especÃ­fico do carrinho do usuÃ¡rio.
        [HttpDelete("itens/{itemId:int}")]
        public async Task<ActionResult> RemoverItemDoCarrinho(int usuarioId, int itemId)
        {
            // Busca o carrinho ativo do usuÃ¡rio.
            var carrinho = await _carrinhoService.ObterOuCriarAtivoPorUsuarioAsync(usuarioId);

            // Busca o item informado pelo id.
            var item = await _itemCarrinhoService.ObterPorIdAsync(itemId);

            // Verifica se o item existe e se pertence ao carrinho do usuÃ¡rio.
            if (item == null || item.CarrinhoId != carrinho.Id)
                return NotFound("Item do carrinho nao encontrado.");

            // Remove o item do carrinho.
            await _itemCarrinhoService.RemoverAsync(itemId);

            // Retorna 204 No Content indicando que a remoÃ§Ã£o foi feita com sucesso.
            return NoContent();
        }
    }
}

