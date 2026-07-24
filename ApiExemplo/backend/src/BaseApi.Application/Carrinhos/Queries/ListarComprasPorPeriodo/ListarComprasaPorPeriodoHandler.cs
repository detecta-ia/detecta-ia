using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;

namespace BaseApi.Application.Carrinhos.Queries.ListarComprasPorPeriodo;

public class ListarComprasPorPeriodoHandler(ICarrinhoRepositorio carrinhoRepositorio)
    : IRequestHandler<ListarComprasPorPeriodoQuery, List<CompraListaDto>>
{
    public async Task<List<CompraListaDto>> Handle(ListarComprasPorPeriodoQuery request, CancellationToken cancellationToken)
    {
        // Normaliza para cobrir o dia inteiro: 00:00:00 da data inicial até 23:59:59.9999999 da data final,
        // assim o filtro funciona corretamente mesmo se o front enviar só a data (sem horário).
        var dataInicial = request.DataInicial.Date;
        var dataFinal = request.DataFinal.Date.AddDays(1).AddTicks(-1);

        var compras = await carrinhoRepositorio.ListarFinalizadasPorPeriodoAsync(
            request.UsuarioId,
            dataInicial,
            dataFinal,
            cancellationToken);

        return compras
            .Select(carrinho => new CompraListaDto(
                carrinho.Id,
                carrinho.AtualizadoEm ?? carrinho.CriadoEm,
                (decimal)carrinho.Itens.Sum(item => item.Quantidade * item.PrecoUnitarioCents) / 100,
                carrinho.Itens.Sum(item => item.Quantidade)
            ))
            .ToList();
    }
}
