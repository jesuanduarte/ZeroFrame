using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Application.DTOS.Pagamento
{
    public class PagamentoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int PedidoId { get; set; }
    }

    public class PagamentoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo Método é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O campo Método deve conter no máximo 50 caracteres.")]
        public string Metodo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo PedidoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O PedidoId deve ser válido.")]
        public int PedidoId { get; set; }
    }

    public class PagamentoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}