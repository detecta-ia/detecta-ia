using BaseApi.Application.Comum.Modelos;
using MediatR;

namespace BaseApi.Application.Compras.Queries.ListarHistoricoCompras;

/// <summary>
/// Lista o histórico de compras (carrinhos finalizados) do usuário autenticado,
/// permitindo que o cliente selecione qualquer uma delas para ver os detalhes. [RN01]
/// </summary>
public record ListarHistoricoComprasQuery(
    Guid UsuarioId,
    int Pagina = 1,
    int TamanhoPagina = 10
) : IRequest<ResultadoPaginado<CompraResumoDto>>;

public record CompraResumoDto(
    Guid CompraId,
    DateTime DataCompra,
    int QuantidadeItens,
    decimal ValorTotal
);
