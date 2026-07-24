using BaseApi.Domain.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BaseApi.Infrastructure.Dados.Configuracoes;

public class CarrinhoConfiguracao : IEntityTypeConfiguration<Carrinho>
{
    public void Configure(EntityTypeBuilder<Carrinho> builder)
    {
        builder.ToTable("Carrinhos");
        builder.HasKey(c => c.Id);

        // Índice de apoio para a listagem de compras por período (filtro por usuário + status + data)
        builder.HasIndex(c => new { c.UsuarioId, c.Status, c.CriadoEm })
            .HasDatabaseName("IX_Carrinhos_UsuarioId_Status_CriadoEm");

        builder.HasMany(c => c.Itens)
            .WithOne()
            .HasForeignKey(i => i.CarrinhoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class CarrinhoItemConfiguracao : IEntityTypeConfiguration<CarrinhoItem>
{
    public void Configure(EntityTypeBuilder<CarrinhoItem> builder)
    {
        builder.ToTable("CarrinhoItens");
        builder.HasKey(i => i.Id);

        // Relacionamento com a tabela de produtos
        builder.HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}