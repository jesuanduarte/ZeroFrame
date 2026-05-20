using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.DTOS
{
    public class SuportePostDto
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        [Required]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public string Assunto { get; set; } = string.Empty;

        [Required]
        public string Mensagem { get; set; } = string.Empty;

        public string Data { get; set; } = string.Empty;
    }
}
