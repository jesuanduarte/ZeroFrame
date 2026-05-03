using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.DTOS.Pedidos;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICarrinhoRepository _carrinhoRepository;
        private readonly IVariacaoRepository _variacaoRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            ICarrinhoRepository carrinhoRepository,
            IVariacaoRepository variacaoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoRepository = carrinhoRepository;
            _variacaoRepository = variacaoRepository;
        }

        public async Task<PedidosGetDto?> ObterPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return null;

            return MapearPedidoGetDto(pedido);
        }

        public async Task<List<PedidosGetDto>> ObterPorUsuarioAsync(int usuarioId)
        {
            var pedidos = await _pedidoRepository.ObterPorUsuarioAsync(usuarioId);
            return pedidos.Select(MapearPedidoGetDto).ToList();
        }

        public async Task<PedidosGetDto> CriarAsync(PedidosPostDto pedidosPostDto)
        {
            var itensPedido = new List<ItemPedido>();

            foreach (var itemDto in pedidosPostDto.Itens)
            {
                var variacao = await _variacaoRepository.ObterPorIdAsync(itemDto.VariacaoProdutoId);

                if (variacao == null)
                    throw new InvalidOperationException("Variacao do produto nao encontrada.");

                if (variacao.Produto == null)
                    throw new InvalidOperationException("Produto da variacao nao encontrado.");

                ValidarEstoque(variacao, itemDto.Quantidade);

                variacao.Estoque -= itemDto.Quantidade;
                await _variacaoRepository.AtualizarAsync(variacao);

                itensPedido.Add(new ItemPedido
                {
                    VariacaoProdutoId = itemDto.VariacaoProdutoId,
                    VariacaoProduto = variacao,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = variacao.Produto.Preco
                });
            }

            var pedido = new Pedidos
            {
                UsuarioId = pedidosPostDto.UsuarioId,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                Itens = itensPedido
            };

            pedido.ValorTotal = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);

            await _pedidoRepository.CriarPedidoAsync(pedido);
            return MapearPedidoGetDto(pedido);
        }

        public async Task<PedidosGetDto> CriarAPartirDoCarrinhoAsync(int carrinhoId)
        {
            var carrinho = await _carrinhoRepository.ObterPorIdAsync(carrinhoId);

            if (carrinho == null)
                throw new InvalidOperationException("Carrinho nao encontrado.");

            if (!carrinho.Ativo)
                throw new InvalidOperationException("Carrinho inativo nao pode gerar pedido.");

            if (!carrinho.Itens.Any())
                throw new InvalidOperationException("Carrinho vazio nao pode gerar pedido.");

            var itensPedido = new List<ItemPedido>();

            foreach (var itemCarrinho in carrinho.Itens)
            {
                var variacao = await _variacaoRepository.ObterPorIdAsync(itemCarrinho.VariacaoProdutoId);

                if (variacao == null)
                    throw new InvalidOperationException("Variacao do produto nao encontrada.");

                if (variacao.Produto == null)
                    throw new InvalidOperationException("Produto da variacao nao encontrado.");

                ValidarEstoque(variacao, itemCarrinho.Quantidade);

                variacao.Estoque -= itemCarrinho.Quantidade;
                await _variacaoRepository.AtualizarAsync(variacao);

                itensPedido.Add(new ItemPedido
                {
                    VariacaoProdutoId = itemCarrinho.VariacaoProdutoId,
                    VariacaoProduto = variacao,
                    Quantidade = itemCarrinho.Quantidade,
                    PrecoUnitario = variacao.Produto.Preco
                });
            }

            var pedido = new Pedidos
            {
                UsuarioId = carrinho.UsuarioId,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                Itens = itensPedido
            };

            pedido.ValorTotal = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
            carrinho.Ativo = false;

            await _pedidoRepository.CriarPedidoAsync(pedido);
            await _carrinhoRepository.AtualizarAsync(carrinho);

            return MapearPedidoGetDto(pedido);
        }

        public async Task<PedidosGetDto> CriarAPartirDoCarrinhoAtivoDoUsuarioAsync(int usuarioId)
        {
            var carrinho = await _carrinhoRepository.ObterAtivoPorUsuarioAsync(usuarioId);

            if (carrinho == null)
                throw new InvalidOperationException("Carrinho ativo nao encontrado para este usuario.");

            return await CriarAPartirDoCarrinhoAsync(carrinho.Id);
        }

        public async Task CancelarAsync(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return;

            if (pedido.Status == "Cancelado")
                return;

            foreach (var item in pedido.Itens)
            {
                var variacao = await _variacaoRepository.ObterPorIdAsync(item.VariacaoProdutoId);

                if (variacao != null)
                {
                    variacao.Estoque += item.Quantidade;
                    await _variacaoRepository.AtualizarAsync(variacao);
                }
            }

            pedido.Status = "Cancelado";
            await _pedidoRepository.AtualizarAsync(pedido);
        }

        public async Task AtualizarAsync(PedidosPutDto pedidosPutDto)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidosPutDto.Id);

            if (pedido == null)
                return;

            pedido.Status = pedidosPutDto.Status;
            await _pedidoRepository.AtualizarAsync(pedido);
        }

        private static void ValidarEstoque(VariacaoProdutos variacao, int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade deve ser maior que zero.");

            if (quantidade > variacao.Estoque)
                throw new InvalidOperationException("Estoque insuficiente para esta variacao.");
        }

        private static PedidosGetDto MapearPedidoGetDto(Pedidos pedido)
        {
            var itens = pedido.Itens.Select(MapearItemPedidoGetDto).ToList();
            var subtotal = itens.Sum(item => item.Subtotal);
            var desconto = 0m;
            var frete = 0m;
            var valorTotal = pedido.ValorTotal > 0 ? pedido.ValorTotal : subtotal - desconto + frete;

            return new PedidosGetDto
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                TotalItens = itens.Sum(item => item.Quantidade),
                Subtotal = subtotal,
                Desconto = desconto,
                Frete = frete,
                ValorTotal = valorTotal,
                Itens = itens
            };
        }

        private static ItemPedidoGetDto MapearItemPedidoGetDto(ItemPedido item)
        {
            var variacao = item.VariacaoProduto;
            var produto = variacao?.Produto;

            return new ItemPedidoGetDto
            {
                Id = item.Id,
                PedidoId = item.PedidoId,
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
