namespace ZeroFrame.Domain.Entidades
{
    public class AvaliacaoProduto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        public decimal Nota { get; set; }
        public string? Comentario { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
