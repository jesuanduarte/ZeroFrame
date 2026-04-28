using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.domain.entidades
{
    public class Pagamento
    {
        public int Id { get; set; }
        
        public string Metodo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int PedidoId { get; set; }
        public Pedidos? Pedido { get; set; }
    }
}
