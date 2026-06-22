using BaseApi.Application.Comum.Modelos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Queries.ListarProdutos;

public record ListarProdutosQuery(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? Busca = null
) : IRequest<ResultadoPaginado<ProdutoListaDto>>;

public record ProdutoListaDto(
    Guid Id,
    string Nome,
    decimal Preco
);
