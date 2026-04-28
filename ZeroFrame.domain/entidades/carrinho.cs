using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.domain.entidades
{
    public class Carrinho
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public bool Ativo { get; set; } = true;

        public List<ItemCarrinho> Itens { get; set; } = new();
    }
}
