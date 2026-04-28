using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    public class ItemCarrinhoService : IItemCarrinhoService
    {
        private readonly IItemCarrinhoRepository _itemCarrinhoRepository;
        private readonly IVariacaoRepository _variacaoRepository;

        public ItemCarrinhoService(
            IItemCarrinhoRepository itemCarrinhoRepository,
            IVariacaoRepository variacaoRepository)
        {
            _itemCarrinhoRepository = itemCarrinhoRepository;
            _variacaoRepository = variacaoRepository;
        }

        public async Task<List<ItemCarrinhoGetDto>> ObterTodosAsync()
        {
            var itens = await _itemCarrinhoRepository.ObterTodosAsync();
            return itens.Select(MapearItemCarrinhoGetDto).ToList();
        }

        public async Task<ItemCarrinhoGetDto?> ObterPorIdAsync(int id)
        {
            var item = await _itemCarrinhoRepository.ObterPorIdAsync(id);

            if (item == null)
                return null;

            return MapearItemCarrinhoGetDto(item);
        }

        public async Task<List<ItemCarrinhoGetDto>> ObterPorCarrinhoAsync(int carrinho)
        {
            var itens = await _itemCarrinhoRepository.ObterPorCarrinhoAsync(carrinho);
            return itens.Select(MapearItemCarrinhoGetDto).ToList();
        }

        public async Task<ItemCarrinhoGetDto> CriarAsync(ItemCarrinhoPostDto itemCarrinhoPostDto)
        {
            var variacao = await _variacaoRepository.ObterPorIdAsync(itemCarrinhoPostDto.VariacaoProdutoId);

            if (variacao == null)
                throw new InvalidOperationException("Variacao do produto nao encontrada.");

            if (variacao.Produto == null)
                throw new InvalidOperationException("Produto da variacao nao encontrado.");

            var itemExistente = await _itemCarrinhoRepository.ObterPorCarrinhoEVariacaoAsync(
                itemCarrinhoPostDto.CarrinhoId,
                itemCarrinhoPostDto.VariacaoProdutoId);

            var quantidadeFinal = itemCarrinhoPostDto.Quantidade + (itemExistente?.Quantidade ?? 0);
            ValidarEstoque(variacao, quantidadeFinal);

            if (itemExistente != null)
            {
                itemExistente.Quantidade = quantidadeFinal;
                itemExistente.PrecoUnitario = variacao.Produto.Preco;

                await _itemCarrinhoRepository.AtualizarAsync(itemExistente);
                return MapearItemCarrinhoGetDto(itemExistente);
            }

            var item = new ItemCarrinho
            {
                CarrinhoId = itemCarrinhoPostDto.CarrinhoId,
                VariacaoProdutoId = itemCarrinhoPostDto.VariacaoProdutoId,
                Quantidade = itemCarrinhoPostDto.Quantidade,
                PrecoUnitario = variacao.Produto.Preco
            };

            await _itemCarrinhoRepository.AdicionarAsync(item);
            return MapearItemCarrinhoGetDto(item);
        }

        public async Task AtualizarAsync(ItemCarrinhoPutDto itemCarrinhoPutDto)
        {
            var item = await _itemCarrinhoRepository.ObterPorIdAsync(itemCarrinhoPutDto.Id);

            if (item == null)
                return;

            var variacao = await _variacaoRepository.ObterPorIdAsync(itemCarrinhoPutDto.VariacaoProdutoId);

            if (variacao == null)
                throw new InvalidOperationException("Variacao do produto nao encontrada.");

            if (variacao.Produto == null)
                throw new InvalidOperationException("Produto da variacao nao encontrado.");

            ValidarEstoque(variacao, itemCarrinhoPutDto.Quantidade);

            item.VariacaoProdutoId = itemCarrinhoPutDto.VariacaoProdutoId;
            item.Quantidade = itemCarrinhoPutDto.Quantidade;
            item.PrecoUnitario = variacao.Produto.Preco;

            await _itemCarrinhoRepository.AtualizarAsync(item);
        }

        public async Task RemoverAsync(int id)
        {
            await _itemCarrinhoRepository.RemoverAsync(id);
        }

        private static void ValidarEstoque(VariacaoProdutos variacao, int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade deve ser maior que zero.");

            if (quantidade > variacao.Estoque)
                throw new InvalidOperationException("Estoque insuficiente para esta variacao.");
        }

        private static ItemCarrinhoGetDto MapearItemCarrinhoGetDto(ItemCarrinho item)
        {
            return new ItemCarrinhoGetDto
            {
                Id = item.Id,
                CarrinhoId = item.CarrinhoId,
                VariacaoProdutoId = item.VariacaoProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            };
        }
    }
}