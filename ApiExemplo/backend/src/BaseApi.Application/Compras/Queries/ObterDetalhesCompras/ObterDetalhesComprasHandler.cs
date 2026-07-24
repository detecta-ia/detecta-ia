using BaseApi.Domain.Excecoes;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;

namespace BaseApi.Application.Compras.Queries.ObterDetalhesCompra;

public class ObterDetalhesCompraHandler(ICarrinhoRepositorio carrinhoRepositorio)
    : IRequestHandler<ObterDetalhesCompraQuery, CompraDetalheDto>
{
    public async Task<CompraDetalheDto> Handle(ObterDetalhesCompraQuery request, CancellationToken cancellationToken)
    {
        var compra = await carrinhoRepositorio.ObterPorIdEUsuarioAsync(request.CompraId, request.UsuarioId, cancellationToken)
            ?? throw new ExcecaoNaoEncontrado("Compra não encontrada.");

        // [RN01] Só é permitido visualizar compras já finalizadas do histórico
        if (compra.Status != "FINALIZADO")
            throw new ExcecaoNaoEncontrado("Compra não encontrada.");

        int totalGeralCents = 0;
        var itensDto = new List<ItemCompraDto>();

        // [RN02] Os itens exibidos são exatamente os registrados no momento da compra
        // (Quantidade e PrecoUnitarioCents ficam "congelados" no CarrinhoItem desde a criação)
        foreach (var item in compra.Itens)
        {
            int subtotalItemCents = item.Quantidade * item.PrecoUnitarioCents;
            totalGeralCents += subtotalItemCents;

            itensDto.Add(new ItemCompraDto(
                item.Id,
                item.ProdutoId,
                item.Produto?.Nome ?? "Produto não identificado",
                item.Quantidade,
                (decimal)item.PrecoUnitarioCents / 100,
                (decimal)subtotalItemCents / 100
            ));
        }

        return new CompraDetalheDto(
            compra.Id,
            compra.AtualizadoEm ?? compra.CriadoEm,
            itensDto,
            (decimal)totalGeralCents / 100
        );
    }
}
