using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Domain.Entidades
{
    public class ItemCarrinho
    {
        public int Id { get; set; }

        public int CarrinhoId { get; set; }
        public Carrinho? Carrinho { get; set; }

        public int VariacaoProdutoId { get; set; }
        public VariacaoProdutos? VariacaoProduto { get; set; }

        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }
    }
}
