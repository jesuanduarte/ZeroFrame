using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.entityconfiguracao
{
    // Configuracao da entidade no banco de dados.

    public class Produtoconfiguracao: IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.Preco)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.PrecoOriginal)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ImagemUrl)
                .HasMaxLength(300);

            builder.Property(x => x.Marca)
                .HasMaxLength(100);

            builder.Property(x => x.Origem)
                .HasMaxLength(100);

           builder.HasOne(x => x.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(x => x.CategoriaId);
        }
    }
}
