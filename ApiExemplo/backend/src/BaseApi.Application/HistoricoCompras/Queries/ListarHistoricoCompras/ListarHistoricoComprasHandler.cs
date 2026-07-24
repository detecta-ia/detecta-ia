using BaseApi.Application.HistoricoCompras.Queries.DTOs;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Queries.ListarHistoricoCompras
{
    public class ListarHistoricoComprasHandler
     : IRequestHandler<ListarHistoricoComprasQuery, List<HistoricoCompraDto>>
    {
        private readonly IHistoricoCompraRepositorio _repository;

        public ListarHistoricoComprasHandler(IHistoricoCompraRepositorio repository)
        {
            _repository = repository;
        }

        public async Task<List<HistoricoCompraDto>> Handle(
            ListarHistoricoComprasQuery request,
            CancellationToken cancellationToken)
        {
            var compras = await _repository.ObterPorUsuarioAsync(request.UsuarioId);

            return compras
                .OrderByDescending(x => x.DataCompra)
                .Select(x => new HistoricoCompraDto
                {
                    Id = x.Id,
                    DataCompra = x.DataCompra,
                    NomeSupermercado = x.NomeSupermercado,
                    QuantidadeItens = x.QuantidadeItens,
                    ValorTotal = x.ValorTotal
                })
                .ToList();
        }
    }
}
