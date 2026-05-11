using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Domain.Entidades
{
    public class Pedidos
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public DateTime DataPedido { get; set; }
        public string Status { get; set; } = string.Empty;

        public decimal ValorTotal { get; set; }

        public List<ItemPedido> Itens { get; set; } = new();

        public Pagamento? Pagamento { get; set; }
    }
}
