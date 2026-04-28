using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.domain.entidades
{
   
   public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public List<Endereco> Enderecos { get; set; } = new();
        public List<Pedidos> Pedidos { get; set; } = new();
    }
}
