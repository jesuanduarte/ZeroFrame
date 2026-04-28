using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.domain.entidades
{
    public class ItemPedido
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }
        public Pedidos? Pedido { get; set; }

        public int VariacaoProdutoId { get; set; }
        public VariacaoProdutos? VariacaoProduto { get; set; }

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }
    }
}
