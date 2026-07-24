using MediatR;

namespace BaseApi.Application.Carrinhos.Queries.ListarComprasPorPeriodo;

/// <summary>
/// Query que retorna as compras (carrinhos finalizados) do usuário autenticado
/// filtradas por um período informado.
/// User Story: "Como cliente, eu quero filtrar minhas compras por período,
/// para que eu encontre informações específicas mais rapidamente."
/// </summary>
/// <remarks>
/// [RN01] Permite filtro por data inicial e final.
/// [RN02] Exibe apenas compras dentro do período informado.
/// </remarks>
public record ListarComprasPorPeriodoQuery(
    Guid UsuarioId,
    DateTime DataInicial,
    DateTime DataFinal
) : IRequest<List<CompraListaDto>>;

public record CompraListaDto(
    Guid Id,
    DateTime Data,
    decimal Total,
    int QuantidadeItens
);
