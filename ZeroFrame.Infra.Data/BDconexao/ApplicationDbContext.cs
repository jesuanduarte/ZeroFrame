using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ZeroFrame.Infra.Data.BDconexao
{
    // Contexto do banco de dados da aplicação.
    // Responsável por representar as tabelas e aplicar as configurações das entidades.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<domain.entidades.Carrinho> carrinhos { get; set; }
        public DbSet<domain.entidades.Categoria> categorias { get; set; }
        public DbSet<domain.entidades.Endereco> enderecos { get; set; }
        public DbSet<domain.entidades.ItemCarrinho> item_Carrinhos { get; set; }
        public DbSet<domain.entidades.ItemPedido> itemPedidos { get; set; }
        public DbSet<domain.entidades.Pagamento> Pagamentos { get; set; }
        public DbSet<domain.entidades.Pedidos> pedidos { get; set; }
        public DbSet<domain.entidades.Produto> produtos { get; set; }
        public DbSet<domain.entidades.Usuario> Usuarios { get; set; }
        public DbSet<domain.entidades.VariacaoProdutos> variacaoprodutos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);    
        }
    }
}
