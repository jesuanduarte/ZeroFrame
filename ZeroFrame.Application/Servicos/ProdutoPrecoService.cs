using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.Servicos
{
    // Centraliza a regra de desconto para evitar divergencia entre cadastro, vitrine e pedido.
    public static class ProdutoPrecoService
    {
        private static readonly string[] TiposDescontoPermitidos = ["porcentagem", "fixo", "nenhum"];

        public static decimal CalcularPrecoFinal(decimal preco, string? tipoDesconto, decimal desconto)
        {
            var tipo = NormalizarTipoDesconto(tipoDesconto);

            return tipo switch
            {
                "porcentagem" => preco - (preco * desconto / 100),
                "fixo" => preco - desconto,
                _ => preco
            };
        }

        public static decimal CalcularPrecoFinal(Produto produto)
        {
            return CalcularPrecoFinal(produto.Preco, produto.TipoDesconto, produto.Desconto);
        }

        public static string NormalizarTipoDesconto(string? tipoDesconto)
        {
            var tipo = string.IsNullOrWhiteSpace(tipoDesconto)
                ? "nenhum"
                : tipoDesconto.Trim().ToLowerInvariant();

            return TiposDescontoPermitidos.Contains(tipo) ? tipo : tipo;
        }

        public static void ValidarProduto(decimal preco, decimal precoCusto, string? tipoDesconto, decimal desconto)
        {
            var tipo = NormalizarTipoDesconto(tipoDesconto);

            if (!TiposDescontoPermitidos.Contains(tipo))
                throw new InvalidOperationException("TipoDesconto deve ser porcentagem, fixo ou nenhum.");

            if (precoCusto <= 0)
                throw new InvalidOperationException("PrecoCusto deve ser obrigatorio e maior que zero.");

            if (preco < precoCusto)
                throw new InvalidOperationException("O preco de venda nao pode ser menor que o preco de custo.");

            if (desconto < 0)
                throw new InvalidOperationException("O desconto nao pode ser negativo.");

            if (tipo == "porcentagem" && desconto > 100)
                throw new InvalidOperationException("O desconto percentual nao pode ser maior que 100.");

            var precoFinal = CalcularPrecoFinal(preco, tipo, desconto);

            if (precoFinal < precoCusto)
                throw new InvalidOperationException("O preco final com desconto nao pode ser menor que o preco de custo.");
        }
    }
}
