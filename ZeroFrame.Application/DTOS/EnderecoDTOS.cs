using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.DTOS.Endereco
{
    public class EnderecoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public int UsuarioId { get; set; }
    }

    public class EnderecoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo Rua é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O campo Rua deve conter no máximo 150 caracteres.")]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Número é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O campo Número deve conter no máximo 20 caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cidade é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo Cidade deve conter no máximo 100 caracteres.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Estado é obrigatório.")]
        [MaxLength(2, ErrorMessage = "Use a sigla do estado, exemplo: SP.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo CEP é obrigatório.")]
        [MaxLength(10, ErrorMessage = "O campo CEP deve conter no máximo 10 caracteres.")]
        public string Cep { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo UsuarioId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O UsuarioId deve ser válido.")]
        public int UsuarioId { get; set; }
    }

    public class EnderecoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public int UsuarioId { get; set; }
    }
}