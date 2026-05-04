using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Application.DTOS.ItemPedido
{
    public class ItemPedidoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public int PedidoId { get; set; }
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

    // DTOs para criação itens de pedido, com validação de dados.
    public class PedidoItemPostDto
    {
        [Required(ErrorMessage = "O campo VariacaoProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O VariacaoProdutoId deve ser válido.")]
        public int VariacaoProdutoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    // DTOs para atualização de itens de pedido, com validação de dados.
    public class PedidoItemPutDto
    {
        [Required(ErrorMessage = "O campo VariacaoProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O VariacaoProdutoId deve ser válido.")]
        public int VariacaoProdutoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    // DTOs para criação e atualização de itens de pedido, com validação de dados.
    public class ItemPedidoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo PedidoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O PedidoId deve ser válido.")]
        public int PedidoId { get; set; }

        [Required(ErrorMessage = "O campo VariacaoProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O VariacaoProdutoId deve ser válido.")]
        public int VariacaoProdutoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

    }

    public class ItemPedidoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int VariacaoProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}

