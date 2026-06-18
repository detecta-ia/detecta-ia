using BaseApi.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using BaseApi.Domain.Repositories;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Command.Queries
{
    public class BuscarCaixaStatusHandler
    : IRequestHandler<BuscarCaixaStatusQuery, string>
    {
        private readonly ICaixaStatusRepositorio _repository;

        public BuscarCaixaStatusHandler(
            ICaixaStatusRepositorio repository)
        {
            _repository = repository;
        }

        public async Task<string> Handle(
            BuscarCaixaStatusQuery request,
            CancellationToken cancellationToken)
        {
            var status = await _repository.ObterAtualAsync();

            return status?.Status ?? "Aguardando";
        }
    }
}

