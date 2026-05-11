using ZeroFrame.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.EntityConfiguracao
{
    // Configuração da entidade no banco de dados.

    public class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Senha)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Telefone)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(u => u.Perfil)
                .HasMaxLength(20);
            builder.Property(x => x.Ativo)
                .IsRequired();
        }
    }
}
