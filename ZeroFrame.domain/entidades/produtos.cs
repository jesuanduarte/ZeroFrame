namespace ZeroFrame.domain.entidades
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }
        public bool Ativo { get; set; } = true;
        public Categoria? Categoria { get; set; }
        public List<VariacaoProdutos> VariacoesProdutos { get; set; } = new();
        
    }
}
