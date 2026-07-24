using FluentValidation;

namespace BaseApi.Application.Carrinhos.Queries.ListarComprasPorPeriodo;

public class ListarComprasPorPeriodoValidator : AbstractValidator<ListarComprasPorPeriodoQuery>
{
    public ListarComprasPorPeriodoValidator()
    {
        RuleFor(x => x.DataInicial)
            .NotEqual(default(DateTime)).WithMessage("Data inicial é obrigatória.");

        RuleFor(x => x.DataFinal)
            .NotEqual(default(DateTime)).WithMessage("Data final é obrigatória.");

        // [RN01] Garante que o período informado seja válido (data inicial não pode ser depois da final)
        RuleFor(x => x)
            .Must(x => x.DataInicial.Date <= x.DataFinal.Date)
            .WithMessage("A data inicial não pode ser posterior à data final.")
            .WithName("Periodo");
    }
}
