using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ZeroFrame.Domain.Entidades
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public List<Produto> Produtos { get; set; } = new();

    }
}
