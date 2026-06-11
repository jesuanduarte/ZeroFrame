using System.ComponentModel.DataAnnotations;

namespace ZeroFrame.Application.DTOS.AvaliacaoProduto
{
    public class AvaliacaoProdutoPostDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "O ProdutoId deve ser valido.")]
        public int ProdutoId { get; set; }

        public decimal Nota { get; set; }

        [MaxLength(500, ErrorMessage = "O comentario deve conter no maximo 500 caracteres.")]
        public string? Comentario { get; set; }
    }

    public class AvaliacaoProdutoPutDto
    {
        public decimal Nota { get; set; }

        [MaxLength(500, ErrorMessage = "O comentario deve conter no maximo 500 caracteres.")]
        public string? Comentario { get; set; }
    }

    public class AvaliacaoProdutoGetDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public decimal Nota { get; set; }
        public string? Comentario { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class AvaliacaoResumoDto
    {
        public decimal MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
        public int Quantidade1Estrela { get; set; }
        public int Quantidade2Estrelas { get; set; }
        public int Quantidade3Estrelas { get; set; }
        public int Quantidade4Estrelas { get; set; }
        public int Quantidade5Estrelas { get; set; }
    }

    public class AvaliacaoProdutoComentarioModeracaoDto
    {
        [MaxLength(500, ErrorMessage = "O comentario deve conter no maximo 500 caracteres.")]
        public string? Comentario { get; set; }
    }
}
