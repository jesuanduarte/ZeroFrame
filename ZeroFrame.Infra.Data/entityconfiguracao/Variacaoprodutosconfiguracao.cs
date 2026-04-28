using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.entityconfiguracao
{
    // Configuração da entidade no banco de dados.
    public class Variacaoprodutosconfiguracao : IEntityTypeConfiguration<VariacaoProdutos>
    {
        public void Configure(EntityTypeBuilder<VariacaoProdutos> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Tamanho)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Cor)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Estoque)
                .IsRequired();

            builder.Property(x => x.ProdutoId)
                .IsRequired();

            builder.HasOne(x => x.Produto)
                .WithMany(p => p.VariacoesProdutos)
                .HasForeignKey(x => x.ProdutoId);
        }
    }
}





