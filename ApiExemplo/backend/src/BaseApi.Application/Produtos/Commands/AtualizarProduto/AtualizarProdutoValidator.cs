using FluentValidation;
using BaseApi.Domain.Interfaces.Repositorios;
using BaseApi.Application.Produtos.Commands.AtualizarProduto;

public class AtualizarProdutoValidator : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoValidator(IProdutoRepositorio repositorio)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id do produto é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome do produto é obrigatório.")
            .MaximumLength(150).WithMessage("Nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Categoria)
            .NotEmpty().WithMessage("A categoria do produto é obrigatória.");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("Preço deve ser maior que zero.");
    }
}