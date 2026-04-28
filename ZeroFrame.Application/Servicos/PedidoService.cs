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
            return new PedidosGetDto
            {
                Id = pedido.Id,
                UsuarioId = pedido.UsuarioId,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                ValorTotal = pedido.ValorTotal,
                Itens = pedido.Itens.Select(item => new ItemPedidoGetDto
                {
                    Id = item.Id,
                    VariacaoProdutoId = item.VariacaoProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList()
            };
        }
    }
}