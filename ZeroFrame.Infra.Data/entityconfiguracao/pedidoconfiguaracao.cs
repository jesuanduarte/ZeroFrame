using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.entityconfiguracao
{
    // Configuração da entidade no banco de dados.
    public class Pedidoconfiguaracao : IEntityTypeConfiguration<Pedidos>
    {
        public void Configure(EntityTypeBuilder<Pedidos> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.DataPedido)
                .IsRequired();

            builder.Property(x => x.ValorTotal)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(x => x.UsuarioId);

            builder.HasMany(x => x.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId);

            builder.HasOne(x => x.Pagamento)
                .WithOne(p => p.Pedido)
                .HasForeignKey<Pagamento>(p => p.PedidoId);
        }
    }
}
