using BaseApi.Domain.Entidades;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.CriarProduto
{
    public class CriarProdutoHandler(IProdutoRepositorio repositorio) : IRequestHandler<CriarProdutoCommand, CriarProdutoResposta>
    {
        public async Task<CriarProdutoResposta> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
        {
            var produto = new Produto
            {
                Nome = request.Nome,
                Preco = request.Preco,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow,
            };

            await repositorio.AdicionarAsync(produto, cancellationToken);
            await repositorio.SalvarAsync(cancellationToken);

            return new CriarProdutoResposta(produto.Id, produto.Nome, produto.Preco, produto.Categoria);
        }
    }
}
