using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Infra.Data.EntityConfiguracao
{
    public class AvaliacaoProdutoConfiguracao : IEntityTypeConfiguration<AvaliacaoProduto>
    {
        public void Configure(EntityTypeBuilder<AvaliacaoProduto> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Nota)
                .HasColumnType("decimal(2,1)")
                .IsRequired();

            builder.Property(a => a.Comentario)
                .HasMaxLength(500);

            builder.Property(a => a.Ativo)
                .IsRequired();

            builder.Property(a => a.DataCriacao)
                .IsRequired();

            builder.HasOne(a => a.Usuario)
                .WithMany(u => u.AvaliacoesProdutos)
                .HasForeignKey(a => a.UsuarioId);

            builder.HasOne(a => a.Produto)
                .WithMany(p => p.AvaliacoesProdutos)
                .HasForeignKey(a => a.ProdutoId);

            builder.HasIndex(a => new { a.UsuarioId, a.ProdutoId })
                .IsUnique()
                .HasFilter("[Ativo] = 1");
        }
    }
}
