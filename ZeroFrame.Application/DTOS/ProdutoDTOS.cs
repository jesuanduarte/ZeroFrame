using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Application.DTOS;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Application.DTOS.Produto
{
    public class ProdutoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }
        public bool Ativo { get; set; }
        public List<VariacaoGetDto> Variacoes { get; set; } = new();
    }

    public class ProdutoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O campo Nome deve conter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [MaxLength(300, ErrorMessage = "O campo Descrição deve conter no máximo 300 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Preço é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo CategoriaId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O CategoriaId deve ser válido.")]
        public int CategoriaId { get; set; }
    }

    public class ProdutoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int CategoriaId { get; set; }
        public bool Ativo { get; set; }
    }
}
