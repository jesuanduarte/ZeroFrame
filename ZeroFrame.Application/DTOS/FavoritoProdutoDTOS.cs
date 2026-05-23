namespace ZeroFrame.Application.DTOS.FavoritoProduto
{
    public class FavoritoProdutoGetDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal PrecoFinal { get; set; }
        public DateTime DataCriacao { get; set; }
        public ProdutoFavoritoGetDto Produto { get; set; } = new();
    }

    public class ProdutoFavoritoGetDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal PrecoFinal { get; set; }
    }
}
