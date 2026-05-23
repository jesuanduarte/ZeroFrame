using ZeroFrame.Application.DTOS.Admin;
using ZeroFrame.Application.Interfaces;
using ZeroFrame.Domain.Enums;
using ZeroFrame.Domain.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;

        public AdminDashboardService(IPedidoRepository pedidoRepository, IProdutoRepository produtoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
        }

        public async Task<AdminDashboardGetDto> ObterDashboardAsync()
        {
            var agora = DateTime.UtcNow;
            var hoje = agora.Date;
            var limiteProximos = hoje.AddDays(3);
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            var pedidosFinanceiros = pedidos
                .Where(pedido => pedido.Status != StatusPedido.Cancelado
                    && !pedido.StatusEntrega.Equals("Cancelado", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var itensFinanceiros = pedidosFinanceiros.SelectMany(pedido => pedido.Itens).ToList();
            var itensMensais = pedidosFinanceiros
                .Where(pedido => pedido.DataPedido.Year == agora.Year && pedido.DataPedido.Month == agora.Month)
                .SelectMany(pedido => pedido.Itens)
                .ToList();

            var produtos = await _produtoRepository.ObterTodosAdminAsync();

            return new AdminDashboardGetDto
            {
                PedidosAtrasados = pedidos.Count(pedido => PedidoEstaAtrasado(pedido.PrevisaoEntrega, pedido.StatusEntrega, hoje)),
                PedidosProximos = pedidos.Count(pedido => PedidoEstaProximo(pedido.PrevisaoEntrega, pedido.StatusEntrega, hoje, limiteProximos)),
                PedidosNoPrazo = pedidos.Count(pedido => PedidoEstaNoPrazo(pedido.PrevisaoEntrega, pedido.StatusEntrega, limiteProximos)),
                FaturamentoBrutoTotal = itensFinanceiros.Sum(item => item.PrecoUnitario * item.Quantidade),
                FaturamentoBrutoMensal = itensMensais.Sum(item => item.PrecoUnitario * item.Quantidade),
                LucroLiquidoTotal = itensFinanceiros.Sum(item => (item.PrecoUnitario - item.PrecoCustoUnitario) * item.Quantidade),
                LucroLiquidoMensal = itensMensais.Sum(item => (item.PrecoUnitario - item.PrecoCustoUnitario) * item.Quantidade),
                ProdutosMaisVendidos = ObterProdutosMaisVendidos(itensFinanceiros),
                ProdutosMelhoresAvaliados = produtos
                    .Where(produto => produto.AvaliacoesProdutos.Any())
                    .Select(produto => new ProdutoMelhorAvaliadoGetDto
                    {
                        ProdutoId = produto.Id,
                        Nome = produto.Nome,
                        MediaAvaliacoes = Math.Round((decimal)produto.AvaliacoesProdutos.Average(avaliacao => avaliacao.Nota), 2),
                        TotalAvaliacoes = produto.AvaliacoesProdutos.Count
                    })
                    .OrderByDescending(produto => produto.MediaAvaliacoes)
                    .ThenByDescending(produto => produto.TotalAvaliacoes)
                    .Take(5)
                    .ToList()
            };
        }

        private static List<ProdutoMaisVendidoGetDto> ObterProdutosMaisVendidos(List<ZeroFrame.Domain.Entidades.ItemPedido> itens)
        {
            return itens
                .Where(item => item.VariacaoProduto?.Produto != null)
                .GroupBy(item => item.VariacaoProduto!.Produto!)
                .Select(grupo => new ProdutoMaisVendidoGetDto
                {
                    ProdutoId = grupo.Key.Id,
                    Nome = grupo.Key.Nome,
                    QuantidadeVendida = grupo.Sum(item => item.Quantidade),
                    FaturamentoBruto = grupo.Sum(item => item.PrecoUnitario * item.Quantidade)
                })
                .OrderByDescending(produto => produto.QuantidadeVendida)
                .Take(5)
                .ToList();
        }

        private static bool PedidoEstaAtrasado(DateTime? previsaoEntrega, string statusEntrega, DateTime hoje)
        {
            return previsaoEntrega.HasValue
                && previsaoEntrega.Value.Date < hoje
                && !PedidoEncerrado(statusEntrega);
        }

        private static bool PedidoEstaProximo(DateTime? previsaoEntrega, string statusEntrega, DateTime hoje, DateTime limiteProximos)
        {
            return previsaoEntrega.HasValue
                && previsaoEntrega.Value.Date >= hoje
                && previsaoEntrega.Value.Date <= limiteProximos
                && !PedidoEncerrado(statusEntrega);
        }

        private static bool PedidoEstaNoPrazo(DateTime? previsaoEntrega, string statusEntrega, DateTime limiteProximos)
        {
            return !PedidoEncerrado(statusEntrega)
                && (!previsaoEntrega.HasValue || previsaoEntrega.Value.Date > limiteProximos);
        }

        private static bool PedidoEncerrado(string statusEntrega)
        {
            return statusEntrega.Equals("Entregue", StringComparison.OrdinalIgnoreCase)
                || statusEntrega.Equals("Cancelado", StringComparison.OrdinalIgnoreCase);
        }
    }
}
