namespace ZeroFrame.Domain.Entidades
{
    public class FavoritoProduto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
