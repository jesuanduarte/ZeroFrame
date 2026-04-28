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
        public int VariacaoProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }

    public class ItemPedidoPostDto
    {
        //POST — Criar dados
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
        public int VariacaoProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}

