using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.domain.entidades;
using ZeroFrame.domain.Interface;

namespace ZeroFrame.Application.Servicos
{
    // Serviço responsável pelas regras de negócio do ItemPedido.
    public class ItemPedidoService : IItemPedidoService
    {
        private readonly IItemPedidoRepository _itemPedidoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IVariacaoRepository _variacaoRepository;

        public ItemPedidoService(
            IItemPedidoRepository itemPedidoRepository,
            IPedidoRepository pedidoRepository,
            IVariacaoRepository variacaoRepository)
        {
            _itemPedidoRepository = itemPedidoRepository;
            _pedidoRepository = pedidoRepository;
            _variacaoRepository = variacaoRepository;
        }

        // Busca um item do pedido por id.
        public async Task<ItemPedidoGetDto?> ObterPorIdAsync(int id)
        {
            var item = await _itemPedidoRepository.ObterPorIdAsync(id);

            if (item == null)
                return null;

            return MapearItemPedidoGetDto(item);
        }

        // Busca todos os itens de um pedido específico.
        public async Task<List<ItemPedidoGetDto>> ObterPorPedidoAsync(int pedidoId)
        {
            await ObterPedidoOuFalharAsync(pedidoId);

            var itens = await _itemPedidoRepository.ObterPorPedidoAsync(pedidoId);

            return itens.Select(MapearItemPedidoGetDto).ToList();
        }

        // Cria um novo item do pedido.
        public async Task<ItemPedidoGetDto> CriarAsync(ItemPedidoPostDto dto)
        {
            var pedido = await ObterPedidoOuFalharAsync(dto.PedidoId);
            var variacao = await ObterVariacaoOuFalharAsync(dto.VariacaoProdutoId);

            ValidarQuantidade(dto.Quantidade);
            ValidarEstoque(variacao, dto.Quantidade);

            var item = new ItemPedido
            {
                PedidoId = pedido.Id,
                VariacaoProdutoId = variacao.Id,
                VariacaoProduto = variacao,
                Quantidade = dto.Quantidade,
                PrecoUnitario = variacao.Produto!.Preco
            };

            variacao.Estoque -= dto.Quantidade;

            await _itemPedidoRepository.AdicionarAsync(item);
            await _variacaoRepository.AtualizarAsync(variacao);
            await RecalcularTotalPedidoAsync(pedido.Id);

            return MapearItemPedidoGetDto(item);
        }

        // Atualiza um item do pedido existente.
        public async Task AtualizarAsync(ItemPedidoPutDto dto)
        {
            var item = await _itemPedidoRepository.ObterPorIdAsync(dto.Id);

            if (item == null)
                throw new KeyNotFoundException("Item do pedido nao encontrado");

            var pedido = await ObterPedidoOuFalharAsync(dto.PedidoId);
            var novaVariacao = await ObterVariacaoOuFalharAsync(dto.VariacaoProdutoId);

            if (item.PedidoId != pedido.Id)
                throw new InvalidOperationException("Item nao pertence ao pedido informado.");

            ValidarQuantidade(dto.Quantidade);

            if (item.VariacaoProdutoId == dto.VariacaoProdutoId)
            {
                var diferencaQuantidade = dto.Quantidade - item.Quantidade;
                ValidarEstoque(novaVariacao, diferencaQuantidade);
                novaVariacao.Estoque -= diferencaQuantidade;
            }
            else
            {
                var variacaoAnterior = await ObterVariacaoOuFalharAsync(item.VariacaoProdutoId);
                variacaoAnterior.Estoque += item.Quantidade;
                await _variacaoRepository.AtualizarAsync(variacaoAnterior);

                ValidarEstoque(novaVariacao, dto.Quantidade);
                novaVariacao.Estoque -= dto.Quantidade;
            }

            item.PedidoId = pedido.Id;
            item.VariacaoProdutoId = novaVariacao.Id;
            item.VariacaoProduto = novaVariacao;
            item.Quantidade = dto.Quantidade;
            item.PrecoUnitario = novaVariacao.Produto!.Preco;

            await _variacaoRepository.AtualizarAsync(novaVariacao);
            await _itemPedidoRepository.AtualizarAsync(item);
            await RecalcularTotalPedidoAsync(pedido.Id);
        }

        // Remove um item do pedido.
        public async Task RemoverAsync(int id)
        {
            var item = await _itemPedidoRepository.ObterPorIdAsync(id);

            if (item == null)
                throw new KeyNotFoundException("Item do pedido nao encontrado");

            var variacao = await _variacaoRepository.ObterPorIdAsync(item.VariacaoProdutoId);

            if (variacao != null)
            {
                variacao.Estoque += item.Quantidade;
                await _variacaoRepository.AtualizarAsync(variacao);
            }

            var pedidoId = item.PedidoId;
            await _itemPedidoRepository.RemoverAsync(id);
            await RecalcularTotalPedidoAsync(pedidoId);
        }

        // Busca todos os itens do pedido.
        public async Task<List<ItemPedidoGetDto>> ObterTodosAsync()
        {
            var itens = await _itemPedidoRepository.ObterTodosAsync();

            return itens.Select(MapearItemPedidoGetDto).ToList();
        }

        // Busca itens do pedido por id do produto.
        private async Task<Pedidos> ObterPedidoOuFalharAsync(int pedidoId)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(pedidoId);

            if (pedido == null)
                throw new KeyNotFoundException("Pedido nao encontrado");

            return pedido;
        }

        // Busca variação do produto 
        private async Task<VariacaoProdutos> ObterVariacaoOuFalharAsync(int variacaoProdutoId)
        {
            var variacao = await _variacaoRepository.ObterPorIdAsync(variacaoProdutoId);

            if (variacao == null)
                throw new KeyNotFoundException("Variacao do produto nao encontrada");

            if (variacao.Produto == null)
                throw new InvalidOperationException("Produto da variacao nao encontrado.");

            return variacao;
        }

        // Valida a quantidade do item do pedido.
        private static void ValidarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new InvalidOperationException("A quantidade deve ser maior que zero.");
        }

        // Valida se há estoque suficiente para a variação do produto.
        private static void ValidarEstoque(VariacaoProdutos variacao, int quantidade)
        {
            if (quantidade <= 0)
                return;

            if (quantidade > variacao.Estoque)
                throw new InvalidOperationException("Estoque insuficiente para esta variacao.");
        }

        // Recalcula o valor total do pedido com base nos itens do pedido.
        private async Task RecalcularTotalPedidoAsync(int pedidoId)
        {
            var pedido = await ObterPedidoOuFalharAsync(pedidoId);

            pedido.ValorTotal = pedido.Itens.Sum(item => item.Quantidade * item.PrecoUnitario);
            await _pedidoRepository.AtualizarAsync(pedido);
        }

        // Mapeia a entidade ItemPedido para o DTO ItemPedidoGetDto.
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
                CategoriaNome = produto?.Categoria?.Nome ?? string.Empty,
                Tamanho = variacao?.Tamanho ?? string.Empty,
                Cor = variacao?.Cor ?? string.Empty,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                Subtotal = item.Quantidade * item.PrecoUnitario
            };
        }
    }
}
