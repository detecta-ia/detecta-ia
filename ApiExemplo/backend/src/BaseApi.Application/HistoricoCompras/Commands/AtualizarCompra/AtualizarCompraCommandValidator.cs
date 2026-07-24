using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.AtualizarCompra
{
    public class AtualizarCompraCommandValidator : AbstractValidator<AtualizarCompraCommand>
    {
        public AtualizarCompraCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.NomeSupermercado)
                .NotEmpty();

            RuleFor(x => x.QuantidadeItens)
                .GreaterThan(0);

            RuleFor(x => x.ValorTotal)
                .GreaterThan(0);
        }
    }
}
