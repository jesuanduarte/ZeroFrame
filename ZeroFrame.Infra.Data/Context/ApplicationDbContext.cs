using ZeroFrame.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ZeroFrame.Infra.Data.Context
{
    // Contexto do banco de dados da aplicação.
    // Responsável por representar as tabelas e aplicar as configurações das entidades.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carrinho> carrinhos { get; set; }
        public DbSet<Categoria> categorias { get; set; }
        public DbSet<Endereco> enderecos { get; set; }
        public DbSet<ItemCarrinho> item_Carrinhos { get; set; }
        public DbSet<ItemPedido> itemPedidos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Pedidos> pedidos { get; set; }
        public DbSet<Produto> produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<VariacaoProdutos> variacaoprodutos { get; set; }
        public DbSet<AvaliacaoProduto> AvaliacoesProdutos { get; set; }
        public DbSet<FavoritoProduto> FavoritosProdutos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);    
        }
    }
}
