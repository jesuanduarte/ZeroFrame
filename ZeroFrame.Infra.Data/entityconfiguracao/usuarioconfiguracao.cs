using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.entityconfiguracao
{
    // Configuração da entidade no banco de dados.

    public class Usuarioconfiguracao : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Senha)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Telefone)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(x => x.Ativo)
                .IsRequired();
        }
    }
}
