using BaseApi.Domain.Entidades;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Infrastructure.Dados.Configuracoes
{
    public class HistoricoCompraConfiguration
      : IEntityTypeConfiguration<HistoricoCompra>
    {
        public void Configure(EntityTypeBuilder<HistoricoCompra> builder)
        {
            builder.ToTable("historico_compras");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NomeSupermercado)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ValorTotal)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DataCompra)
                .IsRequired();
        }
    }
}
