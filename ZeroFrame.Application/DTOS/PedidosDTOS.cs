using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.DTOS.Pedidos
{
    public class PedidosGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataPedido { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItens { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Frete { get; set; }
        public decimal ValorTotal { get; set; }
        public List<ItemPedidoGetDto> Itens { get; set; } = new();
    }

    public class PedidosPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo Usuario é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O UsuarioId deve ser válido.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "A lista de itens é obrigatória.")]
        [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos 1 item.")]
        public List<ItemPedidoPostDto> Itens { get; set; } = new();
    }

    public class PedidosPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}