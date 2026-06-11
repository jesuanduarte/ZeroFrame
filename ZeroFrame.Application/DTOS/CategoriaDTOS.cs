using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Application.DTOS.Categoria
{
    public class CategoriaGetDto
    {
        //GET - Buscar/Ler dados
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }

    public class CategoriaPostDto
    {
        //POST - Criar dados
        [Required(ErrorMessage = "O campo Nome e obrigatorio.")]
        [MaxLength(50, ErrorMessage = "O campo Nome deve conter no maximo 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "O campo Descricao deve conter no maximo 300 caracteres.")]
        public string? Descricao { get; set; }
    }

    public class CategoriaPutDto
    {
        //PUT - Atualizar dados
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome e obrigatorio.")]
        [MaxLength(50, ErrorMessage = "O campo Nome deve conter no maximo 50 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "O campo Descricao deve conter no maximo 300 caracteres.")]
        public string? Descricao { get; set; }
    }
}
