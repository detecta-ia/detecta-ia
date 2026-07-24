using BaseApi.Application.Comum.Modelos;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;

namespace BaseApi.Application.Compras.Queries.ListarHistoricoCompras;

public class ListarHistoricoComprasHandler(ICarrinhoRepositorio carrinhoRepositorio)
    : IRequestHandler<ListarHistoricoComprasQuery, ResultadoPaginado<CompraResumoDto>>
{
    public async Task<ResultadoPaginado<CompraResumoDto>> Handle(ListarHistoricoComprasQuery query, CancellationToken ct)
    {
        var (itens, total) = await carrinhoRepositorio.ListarFinalizadasPorUsuarioAsync(
            query.UsuarioId, query.Pagina, query.TamanhoPagina, ct);

        var itensDto = itens.Select(compra =>
        {
            var valorTotal = compra.Itens.Sum(i => (decimal)(i.Quantidade * i.PrecoUnitarioCents) / 100);
            var quantidadeItens = compra.Itens.Sum(i => i.Quantidade);

            return new CompraResumoDto(
                compra.Id,
                compra.AtualizadoEm ?? compra.CriadoEm,
                quantidadeItens,
                valorTotal
            );
        }).ToList();

        return new ResultadoPaginado<CompraResumoDto>
        {
            Itens = itensDto,
            Total = total,
            Pagina = query.Pagina,
            TamanhoPagina = query.TamanhoPagina
        };
    }
}
