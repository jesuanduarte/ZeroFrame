using Microsoft.EntityFrameworkCore;
using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.entityconfiguracao
{
    // Configuração da entidade no banco de dados.
    public class CategoriaConfiguracao: IEntityTypeConfiguration<Categoria> 
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Descricao)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasMany(x => x.Produtos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.CategoriaId);
        }
    }
}
