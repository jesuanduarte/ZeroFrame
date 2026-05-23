namespace ZeroFrame.Domain.Filtros
{
    public class ProdutoFiltro
    {
        public string? Busca { get; set; }
        public string? Categoria { get; set; }
        public string? Marca { get; set; }
        public string? Origem { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }
        public bool IncluirInativos { get; set; }
    }
}
