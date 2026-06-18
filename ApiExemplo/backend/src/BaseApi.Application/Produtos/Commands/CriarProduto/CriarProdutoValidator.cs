using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.CriarProduto
{
    public class CriarProdutoValidator : AbstractValidator<CriarProdutoCommand>
    {
        public CriarProdutoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome do produto é obrigatório.")
                .MaximumLength(100).WithMessage("Nome do produto deve ter no máximo 100 caracteres.");
            RuleFor(x => x.Preco)
                .GreaterThan(0).WithMessage("Preço do produto deve ser maior que zero.");
        }
    }
}
