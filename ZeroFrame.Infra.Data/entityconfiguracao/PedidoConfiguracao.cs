using ZeroFrame.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.EntityConfiguracao
{
    // Configuração da entidade no banco de dados.

    public class PedidoConfiguracao : IEntityTypeConfiguration<Pedidos>
    {
        public void Configure(EntityTypeBuilder<Pedidos> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(x => x.DataPedido)
                .IsRequired();

            builder.Property(x => x.StatusEntrega)
                .IsRequired()
                .HasMaxLength(30)
                .HasDefaultValue("Pendente");

            builder.Property(x => x.ValorTotal)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(x => x.UsuarioId);

            // O pedido guarda o endereco usado na entrega, sem apagar historico se o endereco for removido.
            builder.HasOne(x => x.Endereco)
                .WithMany(e => e.Pedidos)
                .HasForeignKey(x => x.EnderecoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId);

            builder.HasOne(x => x.Pagamento)
                .WithOne(p => p.Pedido)
                .HasForeignKey<Pagamento>(p => p.PedidoId);
        }
    }
}
