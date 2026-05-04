using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Application.DTOS
{
    public class VariacaoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public string Tamanho { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Estoque { get; set; }
        public int ProdutoId { get; set; }
    }

    // DTOs para criação e atualização de variações de produtos, com validação de dados.
    public class VariacaoProdutoPostDto
    {
        [Required(ErrorMessage = "O campo Tamanho é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Tamanho deve conter no máximo 50 caracteres.")]
        public string Tamanho { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cor é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Cor deve conter no máximo 50 caracteres.")]
        public string Cor { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Estoque é obrigatório.")]
        [Range(0, int.MaxValue, ErrorMessage = "O campo Estoque não pode ser negativo.")]
        public int Estoque { get; set; }
    }

    // DTO para atualização de variações de produtos
    public class VariacaoProdutoPutDto
    {
        [Required(ErrorMessage = "O campo Tamanho é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Tamanho deve conter no máximo 50 caracteres.")]
        public string Tamanho { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cor é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Cor deve conter no máximo 50 caracteres.")]
        public string Cor { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "O campo Estoque não pode ser negativo.")]
        public int Estoque { get; set; }
    }

    // DTOs para criação e atualização de variações de produtos
    public class VariacaoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo Tamanho é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Tamanho deve conter no máximo 50 caracteres.")]
        public string Tamanho { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Cor é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Cor deve conter no máximo 50 caracteres.")]
        public string Cor { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Estoque é obrigatório.")]
        [Range(0, int.MaxValue, ErrorMessage = "O campo Estoque não pode ser negativo.")]
        public int Estoque { get; set; }

        [Required(ErrorMessage = "O campo ProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O campo ProdutoId deve ser válido.")]
        public int ProdutoId { get; set; }
    }

    public class VariacaoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public string Tamanho { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Estoque { get; set; }
        public int ProdutoId { get; set; }
    }
}