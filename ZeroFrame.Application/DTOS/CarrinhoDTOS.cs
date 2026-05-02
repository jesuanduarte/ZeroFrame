using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Application.DTOS.ItemCarrinho;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Application.DTOS
{
    public class CarrinhoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public bool Ativo { get; set; }
        public int TotalItens { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Desconto { get; set; }
        public decimal Frete { get; set; }
        public decimal TotalGeral { get; set; }
        public List<ItemCarrinhoGetDto> Itens { get; set; } = new();
    }

    public class CarrinhoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo UsuarioId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O UsuarioId deve ser válido.")]
        public int UsuarioId { get; set; }
    }

    public class CarrinhoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public List<ItemCarrinhoPutDto> Itens { get; set; } = new();
    }
}
