using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Domain.Enums;

namespace ZeroFrame.Domain.Entidades
{
    public class Pedidos
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public int EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        public DateTime DataPedido { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        public string StatusEntrega { get; set; } = "Pendente";
        public DateTime? PrevisaoEntrega { get; set; }
        public DateTime? DataEntrega { get; set; }

        public decimal ValorTotal { get; set; }

        public List<ItemPedido> Itens { get; set; } = new();

        public Pagamento? Pagamento { get; set; }
    }
}
