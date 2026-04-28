using ZeroFrame.domain.entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroFrame.Infra.Data.entityconfiguracao
{
    // Configuração da entidade no banco de dados.
    public class PagamentosConfiguracao : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Metodo)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.PedidoId)
                .IsRequired();

            builder.HasOne(x => x.Pedido)
                .WithOne(p => p.Pagamento)
                .HasForeignKey<Pagamento>(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}