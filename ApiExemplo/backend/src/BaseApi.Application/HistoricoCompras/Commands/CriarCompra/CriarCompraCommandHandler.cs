using MediatR;
using BaseApi.Domain.Entidades;
using BaseApi.Domain.Interfaces.Repositorios;

namespace BaseApi.Application.HistoricoCompras.Commands.CriarCompra;

public class CriarCompraCommandHandler
    : IRequestHandler<CriarCompraCommand, Guid>
{
    private readonly IHistoricoCompraRepositorio _repository;

    public CriarCompraCommandHandler(IHistoricoCompraRepositorio repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CriarCompraCommand request,
        CancellationToken cancellationToken)
    {
        var compra = new HistoricoCompra(
     request.UsuarioId,
     request.NomeSupermercado,
     request.QuantidadeItens,
     request.ValorTotal
 );

        await _repository.AdicionarAsync(compra);

        return compra.Id;
    }
}