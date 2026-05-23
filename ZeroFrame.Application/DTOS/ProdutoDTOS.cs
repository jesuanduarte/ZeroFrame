using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using ZeroFrame.Application.DTOS;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.DTOS.Produto
{
    public class ProdutoGetDto
    {
        //GET - Buscar/Ler dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal PrecoCusto { get; set; }
        public string TipoDesconto { get; set; } = string.Empty;
        public decimal Desconto { get; set; }
        public decimal PrecoFinal { get; set; }
        public decimal? PrecoOriginal { get; set; }
        public bool EmPromocao { get; set; }
        public string ImagemUrl { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public string SecaoVitrine { get; set; } = string.Empty;
        public string TipoTamanho { get; set; } = string.Empty;
        public string TamanhosDisponiveis { get; set; } = string.Empty;
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
        public bool IncluirInativos { get; set; }
    }


    public class ProdutoPostDto
    {
        //POST - Criar dados
        [Required(ErrorMessage = "O campo Nome e obrigatorio.")]
        [MaxLength(100, ErrorMessage = "O campo Nome deve conter no maximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Descricao e obrigatorio.")]
        [MaxLength(300, ErrorMessage = "O campo Descricao deve conter no maximo 300 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Preco e obrigatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preco deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo PrecoCusto e obrigatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preco de custo deve ser maior que zero.")]
        public decimal PrecoCusto { get; set; }

        [MaxLength(20, ErrorMessage = "O campo TipoDesconto deve conter no maximo 20 caracteres.")]
        public string? TipoDesconto { get; set; } = "nenhum";

        [Range(0, double.MaxValue, ErrorMessage = "O desconto nao pode ser negativo.")]
        public decimal Desconto { get; set; }

        [Required(ErrorMessage = "O campo CategoriaId e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O CategoriaId deve ser valido.")]
        public int CategoriaId { get; set; }

        public decimal? PrecoOriginal { get; set; }

        [MaxLength(300, ErrorMessage = "O campo ImagemUrl deve conter no maximo 300 caracteres.")]
        public string? ImagemUrl { get; set; }

        // Arquivo recebido via multipart/form-data. O banco salva apenas o caminho publico em ImagemUrl.
        public IFormFile? ImagemArquivo { get; set; }

        [MaxLength(100, ErrorMessage = "O campo Marca deve conter no maximo 100 caracteres.")]
        public string? Marca { get; set; }

        [MaxLength(100, ErrorMessage = "O campo Origem deve conter no maximo 100 caracteres.")]
        public string? Origem { get; set; }

        [MaxLength(50, ErrorMessage = "O campo Genero deve conter no maximo 50 caracteres.")]
        public string? Genero { get; set; }

        [MaxLength(50, ErrorMessage = "O campo Cor deve conter no maximo 50 caracteres.")]
        public string? Cor { get; set; }

        [MaxLength(100, ErrorMessage = "O campo SecaoVitrine deve conter no maximo 100 caracteres.")]
        public string? SecaoVitrine { get; set; }

        [MaxLength(50, ErrorMessage = "O campo TipoTamanho deve conter no maximo 50 caracteres.")]
        public string? TipoTamanho { get; set; }

        [MaxLength(300, ErrorMessage = "O campo TamanhosDisponiveis deve conter no maximo 300 caracteres.")]
        public string? TamanhosDisponiveis { get; set; }
    }

    public class ProdutoPutDto
    {
        //PUT - Atualizar dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public decimal PrecoCusto { get; set; }
        public string? TipoDesconto { get; set; } = "nenhum";
        public decimal Desconto { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "O CategoriaId deve ser valido.")]
        public int CategoriaId { get; set; }
        public decimal? PrecoOriginal { get; set; }
        public string? ImagemUrl { get; set; }
        // Quando uma nova imagem nao for enviada, a imagem atual do produto e mantida.
        public IFormFile? ImagemArquivo { get; set; }
        public string? Marca { get; set; }
        public string? Origem { get; set; }
        public string? Genero { get; set; }
        public string? Cor { get; set; }
        public string? SecaoVitrine { get; set; }
        public string? TipoTamanho { get; set; }
        public string? TamanhosDisponiveis { get; set; }
        public bool Ativo { get; set; }
    }
}
