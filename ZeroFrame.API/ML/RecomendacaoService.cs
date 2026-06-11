using Microsoft.ML;
using Microsoft.ML.Trainers;
using ZeroFrame.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ZeroFrame.API.ML
{
    public class RecomendacaoService
    {
        private readonly ApplicationDbContext _context;
        private readonly MLContext _mlContext;
        private ITransformer? _modelo;

        public RecomendacaoService(ApplicationDbContext context)
        {
            _context = context;
            _mlContext = new MLContext(seed: 0);
        }

        public async Task TreinarModeloAsync()
        {
            var pedidos = await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                .Where(i => i.Pedido != null && i.VariacaoProduto != null)
                .Select(i => new DadosPedido
                {
                    UsuarioId = (uint)i.Pedido!.UsuarioId,
                    ProdutoId = (uint)i.VariacaoProduto!.ProdutoId,
                    Avaliacao = 1f
                })
                .ToListAsync();

            if (pedidos.Count < 5)
                return;

            var dados = _mlContext.Data.LoadFromEnumerable(pedidos);

            var opcoes = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(DadosPedido.UsuarioId),
                MatrixRowIndexColumnName = nameof(DadosPedido.ProdutoId),
                LabelColumnName = nameof(DadosPedido.Avaliacao),
                NumberOfIterations = 20,
                ApproximationRank = 10
            };

            var pipeline = _mlContext.Recommendation()
                .Trainers.MatrixFactorization(opcoes);

            _modelo = pipeline.Fit(dados);
        }

        public async Task<List<int>> RecomendarProdutosAsync(int usuarioId, int quantidade = 5)
        {
            if (_modelo == null)
                await TreinarModeloAsync();

            if (_modelo == null)
                return new List<int>();

            var todosProdutos = await _context.produtos
                .Select(p => p.Id)
                .ToListAsync();

            var produtosComprados = await _context.itemPedidos
                .Include(i => i.Pedido)
                .Include(i => i.VariacaoProduto)
                .Where(i => i.Pedido!.UsuarioId == usuarioId && i.VariacaoProduto != null)
                .Select(i => i.VariacaoProduto!.ProdutoId)
                .Distinct()
                .ToListAsync();

            var engine = _mlContext.Model
                .CreatePredictionEngine<DadosPedido, PrevisaoRecomendacao>(_modelo);

            var recomendacoes = todosProdutos
                .Where(id => !produtosComprados.Contains(id))
                .Select(prodId => new
                {
                    ProdutoId = prodId,
                    Score = engine.Predict(new DadosPedido
                    {
                        UsuarioId = (uint)usuarioId,
                        ProdutoId = (uint)prodId
                    }).Score
                })
                .OrderByDescending(x => x.Score)
                .Take(quantidade)
                .Select(x => x.ProdutoId)
                .ToList();

            return recomendacoes;
        }
    }
}
