using BaseApi.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Infrastructure.Dados.Configuracoes
{
     public class CaixaStatusConfiguracao : IEntityTypeConfiguration<CaixaStatus>
    {
        public void Configure(EntityTypeBuilder<CaixaStatus> builder)
        {
            builder.ToTable("CaixaStatus");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(x => x.DataAtualizacao)
                   .IsRequired();
        }
    }
}
