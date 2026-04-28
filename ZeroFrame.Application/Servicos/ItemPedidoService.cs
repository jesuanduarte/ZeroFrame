using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio do ItemPedido
    public class ItemPedidoService : IItemPedidoService
    {
        private readonly IItemPedidoRepository _itemPedidoRepository;

        public ItemPedidoService(IItemPedidoRepository itemPedidoRepository)
        {
            _itemPedidoRepository = itemPedidoRepository;
        }

        // Busca um item pelo Id
        public async Task<ItemPedidoGetDto?> ObterPorIdAsync(int id)
        {
            var item = await _itemPedidoRepository.ObterPorIdAsync(id);

            if (item == null)
                return null;

            return new ItemPedidoGetDto
            {
                Id = item.Id,
                VariacaoProdutoId = item.VariacaoProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            };
        }

        // Busca itens por pedido
        public async Task<List<ItemPedidoGetDto>> ObterPorPedidoAsync(int pedidoId)
        {
            var itens = await _itemPedidoRepository.ObterPorPedidoAsync(pedidoId);

            return itens.Select(item => new ItemPedidoGetDto
            {
                Id = item.Id,
                VariacaoProdutoId = item.VariacaoProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            }).ToList();
        }

        // Cria item do pedido
        public async Task<ItemPedidoGetDto> CriarAsync(ItemPedidoPostDto dto)
        {
            var item = new ItemPedido
            {
                VariacaoProdutoId = dto.VariacaoProdutoId,
                Quantidade = dto.Quantidade,
                PrecoUnitario = 0
            };

            await _itemPedidoRepository.AdicionarAsync(item);

            return new ItemPedidoGetDto
            {
                Id = item.Id,
                VariacaoProdutoId = item.VariacaoProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            };
        }

        // Atualiza item
        public async Task AtualizarAsync(ItemPedidoPutDto dto)
        {
            var item = await _itemPedidoRepository.ObterPorIdAsync(dto.Id);

            if (item == null)
                return;

            item.VariacaoProdutoId = dto.VariacaoProdutoId;
            item.Quantidade = dto.Quantidade;

            await _itemPedidoRepository.AtualizarAsync(item);
        }

        // Remove item
        public async Task RemoverAsync(int id)
        {
            await _itemPedidoRepository.RemoverAsync(id);
        }

        public async Task<List<ItemPedidoGetDto>> ObterTodosAsync()
        {
            var itens = await _itemPedidoRepository.ObterTodosAsync();

            return itens.OfType<ItemPedido>().Select(item => new ItemPedidoGetDto
            {
                Id = item.Id,
                VariacaoProdutoId = item.VariacaoProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            }).ToList();
        }
    }
}
