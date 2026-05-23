using System.ComponentModel.DataAnnotations;

namespace ZeroFrame.Application.DTOS.Endereco
{
    public class EnderecoGetDto
    {
        // GET - Buscar/Ler dados do endereco.
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public bool Ativo { get; set; }
        public int UsuarioId { get; set; }
    }

    public class EnderecoPostDto
    {
        // POST - Criar dados do endereco.
        [Required(ErrorMessage = "O campo Rua e obrigatorio.")]
        [MaxLength(150, ErrorMessage = "O campo Rua deve conter no maximo 150 caracteres.")]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Numero e obrigatorio.")]
        [MaxLength(20, ErrorMessage = "O campo Numero deve conter no maximo 20 caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "O campo Bairro deve conter no maximo 100 caracteres.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cidade e obrigatorio.")]
        [MaxLength(100, ErrorMessage = "O campo Cidade deve conter no maximo 100 caracteres.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Estado e obrigatorio.")]
        [MaxLength(2, ErrorMessage = "Use a sigla do estado, exemplo: SP.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo CEP e obrigatorio.")]
        [MaxLength(10, ErrorMessage = "O campo CEP deve conter no maximo 10 caracteres.")]
        public string Cep { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "O campo Telefone deve conter no maximo 20 caracteres.")]
        public string Telefone { get; set; } = string.Empty;

        [MaxLength(150, ErrorMessage = "O campo Complemento deve conter no maximo 150 caracteres.")]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "O campo UsuarioId e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O UsuarioId deve ser valido.")]
        public int UsuarioId { get; set; }
    }

    public class EnderecoPutDto
    {
        // PUT - Atualizar dados do endereco existente.
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public bool? Ativo { get; set; }
        public int UsuarioId { get; set; }
    }
}
