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
        public decimal? PrecoOriginal { get; set; }
        public bool EmPromocao { get; set; }
        public string ImagemUrl { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public List<VariacaoGetDto> Variacoes { get; set; } = new();
    }
    public class ProdutoFiltroDto
    {
        public string? Busca { get; set; }
        public string? Categoria { get; set; }
        public string? Marca { get; set; }
        public string? Origem { get; set; }
        public string? Tamanho { get; set; }
        public string? Cor { get; set; }
        public decimal? PrecoMin { get; set; }
        public decimal? PrecoMax { get; set; }
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
        [Range(1, int.MaxValue, ErrorMessage = "O CategoriaId deve ser válido.")]
        public int CategoriaId { get; set; }
        public bool Ativo { get; set; }
    }
}
