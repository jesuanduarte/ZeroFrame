using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Infra.Data.EntityConfiguracao
{
    public class FavoritoProdutoConfiguracao : IEntityTypeConfiguration<FavoritoProduto>
    {
        public void Configure(EntityTypeBuilder<FavoritoProduto> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.DataCriacao)
                .IsRequired();

            builder.HasOne(f => f.Usuario)
                .WithMany(u => u.FavoritosProdutos)
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Produto)
                .WithMany(p => p.FavoritosProdutos)
                .HasForeignKey(f => f.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(f => new { f.UsuarioId, f.ProdutoId })
                .IsUnique();
        }
    }
}
