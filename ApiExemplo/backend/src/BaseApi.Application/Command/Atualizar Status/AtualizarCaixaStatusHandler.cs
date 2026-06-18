using BaseApi.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Command.Atualizar_Status
{
    public class AtualizarCaixaStatusHandler
     : IRequestHandler<AtualizarCaixaStatusCommand, bool>
    {
        private readonly ICaixaStatusRepositorio _repository;

        public AtualizarCaixaStatusHandler(
            ICaixaStatusRepositorio repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(
            AtualizarCaixaStatusCommand request,
            CancellationToken cancellationToken)
        {
            var status = await _repository.ObterAtualAsync();

            if (status == null)
            {
                status = new Domain.Entidades.CaixaStatus(request.Status);

                await _repository.CriarAsync(status);

                return true;
            }

            status.AtualizarStatus(request.Status);

            await _repository.AtualizarAsync(status);

            return true;
        }
    }
}
