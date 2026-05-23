using ZeroFrame.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using ZeroFrame.Application.DTOS.Endereco;

namespace ZeroFrame.Application.DTOS.Usuario
{
    public class UsuarioGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public List<EnderecoGetDto> Enderecos { get; set; } = new();
    }

    public class UsuarioAdminGetDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public string Perfil { get; set; } = string.Empty;
        public int QuantidadePedidos { get; set; }
    }

    // DTO para criação de usuário, com validação de dados
    public class UsuarioPostDto
    {
        
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Nome deve conter no máximo 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        [MaxLength(100, ErrorMessage = "O campo Email deve conter no máximo 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [MinLength(6, ErrorMessage = "A senha deve conter no mínimo 6 caracteres.")]
        [MaxLength(50, ErrorMessage = "A senha deve conter no máximo 50 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Telefone é obrigatório.")]
        [MaxLength(20, ErrorMessage = "O campo Telefone deve conter no máximo 20 caracteres.")]
        public string Telefone { get; set; } = string.Empty;

    }

    // DTO para cadastro simples de usuário, sem nome e telefone, apenas email e senha, com validação de dados
    public class UsuarioCadastroSimplesDto
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        [MaxLength(100, ErrorMessage = "O campo Email deve conter no máximo 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [MinLength(6, ErrorMessage = "A senha deve conter no mínimo 6 caracteres.")]
        [MaxLength(50, ErrorMessage = "A senha deve conter no máximo 50 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo ConfirmarSenha é obrigatório.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }

    // DTO para login de usuário, apenas email e senha, com validação de dados
    public class UsuarioLoginDto
    {
        [Required(ErrorMessage = "O campo Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo Email deve ser um endereço de email válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        public string Senha { get; set; } = string.Empty;
    }

    // DTO para resposta de login de usuário, com os dados do usuário logado
    public class UsuarioLoginResponseDto
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public string Perfil { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    // DTO para atualização de usuário, com validação de dados
    public class UsuarioPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
    }

}
