namespace ZeroFrame.Domain.Entidades
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal PrecoCusto { get; set; }
        public decimal? PrecoOriginal { get; set; }
        public string TipoDesconto { get; set; } = "nenhum";
        public decimal Desconto { get; set; }
        public string Genero { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public string SecaoVitrine { get; set; } = string.Empty;
        public string TipoTamanho { get; set; } = string.Empty;
        public string TamanhosDisponiveis { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public bool Ativo { get; set; } = true;
        public Categoria? Categoria { get; set; }
        public List<VariacaoProdutos> VariacoesProdutos { get; set; } = new();
        public List<AvaliacaoProduto> AvaliacoesProdutos { get; set; } = new();
        public List<FavoritoProduto> FavoritosProdutos { get; set; } = new();
        
    }
}
