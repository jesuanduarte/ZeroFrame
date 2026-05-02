using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.domain.entidades;
using ZeroFrame.Application.Interfaces;
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
            item.VariacaoProduto = variacao;
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
            var variacao = item.VariacaoProduto;
            var produto = variacao?.Produto;

            return new ItemCarrinhoGetDto
            {
                Id = item.Id,
                CarrinhoId = item.CarrinhoId,
                VariacaoProdutoId = item.VariacaoProdutoId,
                ProdutoId = produto?.Id ?? 0,
                NomeProduto = produto?.Nome ?? string.Empty,
                ImagemUrl = produto == null ? string.Empty : ObterImagemUrl(produto),
                CategoriaNome = produto?.Categoria?.Nome ?? string.Empty,
                Marca = produto == null ? string.Empty : ObterMarca(produto),
                Origem = produto == null ? string.Empty : ObterOrigem(produto),
                Tamanho = variacao?.Tamanho ?? string.Empty,
                Cor = variacao?.Cor ?? string.Empty,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Subtotal = item.Quantidade * item.PrecoUnitario
            };
        }

        private static string ObterImagemUrl(Produto produto)
        {
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("jordan") || nomeNormalizado.Contains("latte"))
                return "/assets/products/aj1-high-latte.png";

            if (nomeNormalizado.Contains("camisa") || nomeNormalizado.Contains("oversized"))
                return "/assets/products/camisa-over-black.png";

            if (nomeNormalizado.Contains("bermuda"))
                return "/assets/products/bermuda-jeans.jpg";

            if (nomeNormalizado.Contains("moletom") || nomeNormalizado.Contains("blusa"))
                return "/assets/products/blusa-moletom.jpg";

            if (nomeNormalizado.Contains("calca") || nomeNormalizado.Contains("calça") || nomeNormalizado.Contains("jeans"))
                return "/assets/products/calca-levis-clara.png";

            if (nomeNormalizado.Contains("corrente") || nomeNormalizado.Contains("ice"))
                return "/assets/products/corrente-ice.png";

            if (nomeNormalizado.Contains("adidas") || nomeNormalizado.Contains("tenis") || nomeNormalizado.Contains("tênis"))
                return "/assets/products/tenis2.png";

            return "/assets/products/camisa-over-black.png";
        }

        private static string ObterMarca(Produto produto)
        {
            var nomeNormalizado = produto.Nome.ToLowerInvariant();

            if (nomeNormalizado.Contains("nike") || nomeNormalizado.Contains("jordan"))
                return "Nike";

            if (nomeNormalizado.Contains("adidas"))
                return "Adidas";

            if (nomeNormalizado.Contains("levis") || nomeNormalizado.Contains("levi"))
                return "Levi's";

            return "Zero Frame";
        }

        private static string ObterOrigem(Produto produto)
        {
            return ObterMarca(produto) == "Zero Frame" ? "Original" : "Multimarcas";
        }
    }
}