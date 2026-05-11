using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.Domain.Entidades;

namespace ZeroFrame.Infra.Data.EntityConfiguracao
{
    // Configuração da entidade no banco de dados.
    public class ItemPedidoConfiguracao : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantidade)
                .IsRequired();

            builder.Property(x => x.PrecoUnitario)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(x => x.PedidoId);

            builder.HasOne(x => x.VariacaoProduto)
                .WithMany()
                .HasForeignKey(x => x.VariacaoProdutoId);
        }
    }
}

