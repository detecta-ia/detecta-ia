using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Queries.ObterProdutoPorId;

public record ObterProdutoPorIdQuery(Guid Id) : IRequest<ProdutoDetalheDto>;

public record ProdutoDetalheDto(
    Guid Id,
    string Nome,
    decimal Preco,
    DateTime CriadoEm,
    DateTime AtualizadoEm
);

