using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.DTOS.ItemCarrinho
{
    public class ItemCarrinhoGetDto
    {
        //GET - Buscar/Ler dados
        public int Id { get; set; }
        public int CarrinhoId { get; set; }
        public int VariacaoProdutoId { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string ImagemUrl { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Tamanho { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ItemCarrinhoPostDto
    {
        //POST - Criar dados
        [Required(ErrorMessage = "O campo Carrinho e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Carrinho deve ser valido.")]
        public int CarrinhoId { get; set; }

        [Required(ErrorMessage = "O campo VariacaoProdutoId e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O VariacaoProdutoId deve ser valido.")]
        public int VariacaoProdutoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    public class ItemCarrinhoUsuarioPostDto
    {
        [Required(ErrorMessage = "O campo VariacaoProdutoId e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O VariacaoProdutoId deve ser valido.")]
        public int VariacaoProdutoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    public class ItemCarrinhoPutDto
    {
        //PUT - Atualizar dados
        public int Id { get; set; }
        public int VariacaoProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}
