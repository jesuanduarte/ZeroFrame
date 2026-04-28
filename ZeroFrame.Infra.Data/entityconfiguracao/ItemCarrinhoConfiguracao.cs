using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using ZeroFrame.domain.entidades;

namespace ZeroFrame.Infra.Data.entityconfiguracao
{
    // Configuração da entidade no banco de dados.
    public class ItemCarrinhoConfiguracao: IEntityTypeConfiguration<ItemCarrinho>
    {
        public void Configure(EntityTypeBuilder<ItemCarrinho> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantidade)
                .IsRequired();

            builder.Property(x => x.CarrinhoId)
                .IsRequired();

            builder.Property(x => x.VariacaoProdutoId)
                .IsRequired();

            builder.Property(x => x.PrecoUnitario)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Carrinho)
                .WithMany(c => c.Itens)
                .HasForeignKey(x => x.CarrinhoId);

            builder.HasOne(x => x.VariacaoProduto)
                .WithMany()
                .HasForeignKey(x => x.VariacaoProdutoId);
        }

    }
}
