using MediatR;

namespace BaseApi.Application.Carrinhos.Commands.FinalizarCompra;

public record FinalizarCompraCommand(Guid UsuarioId) : IRequest<FinalizarCompraResposta>;

public record FinalizarCompraResposta(
    Guid CompraId,
    DateTime DataCompra,
    decimal ValorTotal
);