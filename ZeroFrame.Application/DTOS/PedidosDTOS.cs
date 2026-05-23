using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ZeroFrame.Application.DTOS.ItemPedido;
using ZeroFrame.Domain.Enums;

namespace ZeroFrame.Application.DTOS.Pedidos
{
    public class PedidosGetDto
    {
        // GET - Buscar/Ler dados do pedido, incluindo o endereco de entrega usado no checkout.
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public PedidoUsuarioGetDto Usuario { get; set; } = new();
        public int EnderecoId { get; set; }
        public PedidoEnderecoGetDto Endereco { get; set; } = new();
        public DateTime DataPedido { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusPedido Status { get; set; }
        public string MensagemStatus { get; set; } = string.Empty;
        public string StatusEntrega { get; set; } = string.Empty;
        public DateTime? PrevisaoEntrega { get; set; }
        public DateTime? DataEntrega { get; set; }
        public int TotalItens { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Frete { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorTotalComFrete { get; set; }
        public List<ItemPedidoGetDto> Itens { get; set; } = new();
    }

    public class PedidosPostDto
    {
        // POST - Criar pedido informando explicitamente o endereco de entrega.
        [Required(ErrorMessage = "O campo Usuario e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O UsuarioId deve ser valido.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O campo EnderecoId e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O EnderecoId deve ser valido.")]
        public int EnderecoId { get; set; }

        [Required(ErrorMessage = "A lista de itens e obrigatoria.")]
        [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos 1 item.")]
        public List<ItemPedidoPostDto> Itens { get; set; } = new();
    }

    public class PedidoCarrinhoPostDto
    {
        [Required(ErrorMessage = "O campo EnderecoId e obrigatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "O EnderecoId deve ser valido.")]
        public int EnderecoId { get; set; }
    }

    public class PedidoEnderecoGetDto
    {
        public int EnderecoId { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string? Complemento { get; set; }
    }

    public class PedidoUsuarioGetDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class PedidoStatusEntregaUpdateDto
    {
        [Required(ErrorMessage = "O campo StatusEntrega e obrigatorio.")]
        public string StatusEntrega { get; set; } = string.Empty;
        public DateTime? PrevisaoEntrega { get; set; }
    }

    public class PedidoStatusUpdateDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusPedido Status { get; set; }
    }
}
