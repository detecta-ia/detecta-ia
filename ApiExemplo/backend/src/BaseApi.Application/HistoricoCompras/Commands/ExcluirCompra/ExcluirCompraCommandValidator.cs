using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.ExcluirCompra
{
    public class ExcluirCompraCommandValidator : AbstractValidator<ExcluirCompraCommand>
    {
        public ExcluirCompraCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
