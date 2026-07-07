using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.CriarProduto
{
    public record CriarProdutoCommand
        (
        string Nome,
        decimal Preco,
        string Categoria,
        string? ImagemBase64 = null
    ) : IRequest<CriarProdutoResposta>;

    public record CriarProdutoResposta(
        Guid Id,
        string Nome,
        decimal Preco,
        string Categoria

    );
}