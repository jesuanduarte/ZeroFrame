using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Application.DTOS.ItemCarrinho
{
    public class ItemCarrinhoGetDto
    {
        //GET — Buscar/Ler dados
        public int Id { get; set; }
        public int CarrinhoId { get; set; }
        public int VariacaoProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }

    public class ItemCarrinhoPostDto
    {
        //POST — Criar dados
        [Required(ErrorMessage = "O campo Carrinho é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O Carrinho deve ser válido.")]
        public int CarrinhoId { get; set; }

        [Required(ErrorMessage = "O campo VariacaoProdutoId é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O VariacaoProdutoId deve ser válido.")]
        public int VariacaoProdutoId { get; set; }

        [Required(ErrorMessage = "O campo Quantidade é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    public class ItemCarrinhoPutDto
    {
        //PUT — Atualizar dados
        public int Id { get; set; }
        public int VariacaoProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}
