using BaseApi.Domain.Excecoes;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;

namespace BaseApi.Application.Carrinhos.Queries.ObterCarrinhoRevisao;

public class ObterCarrinhoRevisaoHandler(ICarrinhoRepositorio carrinhoRepositorio)
    : IRequestHandler<ObterCarrinhoRevisaoQuery, CarrinhoRevisaoDto>
{
    public async Task<CarrinhoRevisaoDto> Handle(ObterCarrinhoRevisaoQuery request, CancellationToken cancellationToken)
    {
        var carrinho = await carrinhoRepositorio.ObterAtivoPorUsuarioIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new ExcecaoNaoEncontrado("Nenhum carrinho ativo encontrado para este usuário.");

        int totalGeralCents = 0;
        var listaItensDto = new List<ItemRevisaoDto>();

        foreach (var item in carrinho.Itens)
        {
            string nomeProduto = item.Produto?.Nome ?? "Produto Identificado";
            string imagemUrl = item.Produto?.ImagemUrl ?? "https://cdn.scaniq.com/placeholder.png";

            int subtotalItemCents = item.Quantidade * item.PrecoUnitarioCents;
            totalGeralCents += subtotalItemCents;

            listaItensDto.Add(new ItemRevisaoDto(
                item.Id,
                item.ProdutoId,
                nomeProduto,
                item.Quantidade,
                (decimal)item.PrecoUnitarioCents / 100,
                (decimal)subtotalItemCents / 100,
                imagemUrl
            ));
        }

        // [RN01] Agrupa por categoria mapeada na entidade Produto
        var categoriasAgrupadas = listaItensDto
            .GroupBy(i => carrinho.Itens.First(orig => orig.Id == i.Id).Produto?.Categoria ?? "Outros")
            .Select(g => new GrupoCategoriaDto(g.Key, g.ToList()))
            .ToList();

        return new CarrinhoRevisaoDto(
            carrinho.Id,
            (decimal)totalGeralCents / 100,
            categoriasAgrupadas
        );
    }
}