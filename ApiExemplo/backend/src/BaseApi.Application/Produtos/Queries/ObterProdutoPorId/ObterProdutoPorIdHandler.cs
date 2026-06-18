using BaseApi.Domain.Interfaces.Repositorios;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Queries.ObterProdutoPorId;

public class ObterProdutoPorIdHandler(IProdutoRepositorio repositorio) : IRequestHandler<ObterProdutoPorIdQuery, ProdutoDetalheDto>
{
    public async Task<ProdutoDetalheDto> Handle(ObterProdutoPorIdQuery query, CancellationToken ct)
    {
        var produto = await repositorio.ObterPorIdAsync(query.Id, ct)
            ?? throw new Exception($"Produto com Id '{query.Id}' não encontrado.");
        return produto.Adapt<ProdutoDetalheDto>();
    }
}
