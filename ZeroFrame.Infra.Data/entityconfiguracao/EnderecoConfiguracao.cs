using ZeroFrame.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;


namespace ZeroFrame.Infra.Data.EntityConfiguracao
{
    // Configuração da entidade no banco de dados.

    public class EnderecoConfiguracao : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Rua)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Numero)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Bairro)
                .HasMaxLength(100);

            builder.Property(x => x.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Estado)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CEP)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Complemento)
                .HasMaxLength(150);

            builder.Property(x => x.UsuarioId)
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithMany(u => u.Enderecos)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 



