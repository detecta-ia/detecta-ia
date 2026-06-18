using BaseApi.Application.Comum.Modelos;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Queries.ListarProdutos;

public class ListarProdutosHandler(IProdutoRepositorio repositorio) : IRequestHandler<ListarProdutosQuery, ResultadoPaginado<ProdutoListaDto>>
{
    public async Task<ResultadoPaginado<ProdutoListaDto>> Handle(ListarProdutosQuery query, CancellationToken ct)
    {
        var (itens, total) = await repositorio.ListarAsync(query.Pagina, query.TamanhoPagina, query.Busca, ct);
        return new ResultadoPaginado<ProdutoListaDto>
        {
            Itens = itens.Select(p => new ProdutoListaDto(p.Id, p.Nome, p.Preco)).ToList(),
            Total = total,
            Pagina = query.Pagina,
            TamanhoPagina = query.TamanhoPagina
        };
    }
}