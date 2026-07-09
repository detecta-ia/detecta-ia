using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.AtualizarCompra
{
    public class AtualizarCompraCommandHandler
     : IRequestHandler<AtualizarCompraCommand>
    {
        private readonly IHistoricoCompraRepositorio _repository;

        public AtualizarCompraCommandHandler(IHistoricoCompraRepositorio repository)
        {
            _repository = repository;
        }

        public async Task Handle(
            AtualizarCompraCommand request,
            CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.Id);

            if (compra == null)
                throw new Exception("Compra não encontrada.");

            compra.Atualizar(
    request.NomeSupermercado,
    request.DataCompra,
    request.QuantidadeItens,
    request.ValorTotal
);

            await _repository.AtualizarAsync(compra);
        }
    }
}
