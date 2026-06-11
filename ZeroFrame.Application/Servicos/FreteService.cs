using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.Application.Servicos
{
    // Centraliza as faixas de frete para carrinho, pedido e pagamento usarem a mesma regra.
    public class FreteService : IFreteService
    {
        private const decimal PercentualAteCinquenta = 0.30m;
        private const decimal PercentualAteCem = 0.25m;
        private const decimal PercentualAteCentoECinquenta = 0.20m;
        private const decimal LimitePrimeiraFaixa = 50.00m;
        private const decimal LimiteSegundaFaixa = 100.00m;
        private const decimal LimiteFreteGratis = 150.00m;

        public decimal CalcularFrete(decimal valorTotalCompra)
        {
            if (valorTotalCompra <= 0)
                return 0m;

            var percentual = valorTotalCompra switch
            {
                <= LimitePrimeiraFaixa => PercentualAteCinquenta,
                <= LimiteSegundaFaixa => PercentualAteCem,
                <= LimiteFreteGratis => PercentualAteCentoECinquenta,
                _ => 0m
            };

            var valorFrete = valorTotalCompra * percentual;

            // Arredonda para moeda e garante que nenhuma combinacao gere frete negativo.
            return Math.Max(0m, Math.Round(valorFrete, 2, MidpointRounding.AwayFromZero));
        }
    }
}
