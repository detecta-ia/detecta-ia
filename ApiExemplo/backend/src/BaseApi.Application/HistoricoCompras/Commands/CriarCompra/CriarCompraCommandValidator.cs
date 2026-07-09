using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.CriarCompra
{
    public class CriarCompraCommandValidator : AbstractValidator<CriarCompraCommand>
    {
        public CriarCompraCommandValidator()
        {
            RuleFor(x => x.NomeSupermercado)
                .NotEmpty();

            RuleFor(x => x.QuantidadeItens)
                .GreaterThan(0);

            RuleFor(x => x.ValorTotal)
                .GreaterThan(0);
        }
    }
}
