using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.domain.entidades
{
    public class Endereco
    {
        public int Id { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public int UsuarioId { get; set; }
        public List<Endereco> Enderecos { get; set; } = new();
        public Usuario? Usuario { get; set; }
    }

}
