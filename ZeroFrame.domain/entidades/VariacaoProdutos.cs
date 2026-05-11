using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Domain.Entidades
{
    public class VariacaoProdutos
    {

        public int Id { get; set; }
        public string Tamanho { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Estoque { get; set; } // O controle de estoque fica aqui
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }

    }
}
