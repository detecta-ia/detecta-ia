using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.ExcluirProduto;

public class ExcluirProdutoHandler(IProdutoRepositorio repositorio) : IRequestHandler<ExcluirProdutoCommand, Unit>
{
    public async Task<Unit> Handle(ExcluirProdutoCommand command, CancellationToken ct)
    {
        var produto = await repositorio.ObterPorIdAsync(command.Id, ct)
            ?? throw new Exception($"Produto com Id '{command.Id}' não encontrado.");
        repositorio.Remover(produto);
        await repositorio.SalvarAsync(ct);
        return Unit.Value;
    }
}
