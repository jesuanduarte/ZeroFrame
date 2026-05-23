using System.ComponentModel.DataAnnotations;

namespace ZeroFrame.Application.DTOS.Usuario
{
    public class UsuarioPerfilPutDto
    {
        [Required(ErrorMessage = "O campo Nome e obrigatorio.")]
        [MaxLength(50, ErrorMessage = "O campo Nome deve conter no maximo 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Telefone e obrigatorio.")]
        [MaxLength(20, ErrorMessage = "O campo Telefone deve conter no maximo 20 caracteres.")]
        public string Telefone { get; set; } = string.Empty;
    }
}
