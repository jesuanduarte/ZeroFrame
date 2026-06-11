namespace ZeroFrame.Application.DTOS.Admin
{
    public class AdminDashboardGetDto
    {
        public int PedidosAtrasados { get; set; }
        public int PedidosProximos { get; set; }
        public int PedidosNoPrazo { get; set; }
        public decimal FaturamentoBrutoTotal { get; set; }
        public decimal FaturamentoBrutoMensal { get; set; }
        public decimal LucroLiquidoTotal { get; set; }
        public decimal LucroLiquidoMensal { get; set; }
        public List<ProdutoMaisVendidoGetDto> ProdutosMaisVendidos { get; set; } = new();
        public List<ProdutoMelhorAvaliadoGetDto> ProdutosMelhoresAvaliados { get; set; } = new();
    }

    public class ProdutoMaisVendidoGetDto
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal FaturamentoBruto { get; set; }
    }

    public class ProdutoMelhorAvaliadoGetDto
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
    }
}
