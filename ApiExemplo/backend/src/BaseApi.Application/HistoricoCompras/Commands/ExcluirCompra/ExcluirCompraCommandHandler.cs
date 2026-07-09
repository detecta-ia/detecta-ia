using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.ExcluirCompra
{
    public class ExcluirCompraCommandHandler
     : IRequestHandler<ExcluirCompraCommand>
    {
        private readonly IHistoricoCompraRepositorio _repository;

        public ExcluirCompraCommandHandler(IHistoricoCompraRepositorio repository)
        {
            _repository = repository;
        }

        public async Task Handle(
            ExcluirCompraCommand request,
            CancellationToken cancellationToken)
        {
            await _repository.RemoverAsync(request.Id);
        }
    }
}
