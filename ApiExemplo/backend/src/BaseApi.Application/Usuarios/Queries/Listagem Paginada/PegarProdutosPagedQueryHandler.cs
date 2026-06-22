using BaseApi.Application.Comum.lista;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Usuarios.Queries.Listagem_Paginada
{
    public class PegarProdutosPagedQueryHandler
     : IRequestHandler<PegarProdutosPagedQuery, PagedResult<ProdutoDto>>
    {
        private readonly IProdutoRepository _repository;

        public PegarProdutosPagedQueryHandler(
            IProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<ProdutoDto>> Handle(
            PegarProdutosPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (produtos, total) =
                await _repository.GetPagedAsync(
                    request.PageNumber,
                    request.PageSize);

            return new PagedResult<ProdutoDto>
            {
                Items = produtos.Select(x => new ProdutoDto
                {
                    Id = x.Id,
                    Nome = x.Nome,
                    Preco = x.Preco
                }),

                TotalItems = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
