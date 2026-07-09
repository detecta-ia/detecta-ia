using BaseApi.Application.HistoricoCompras.Queries.DTOs;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Queries.BuscarCompraPorId
{
    public class BuscarCompraPorIdHandler
     : IRequestHandler<BuscarCompraPorIdQuery, HistoricoCompraDto?>
    {
        private readonly IHistoricoCompraRepositorio _repository;

        public BuscarCompraPorIdHandler(IHistoricoCompraRepositorio repository)
        {
            _repository = repository;
        }

        public async Task<HistoricoCompraDto?> Handle(
            BuscarCompraPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var compra = await _repository.ObterPorIdAsync(request.Id);

            if (compra == null)
                return null;

            return new HistoricoCompraDto
            {
                Id = compra.Id,
                DataCompra = compra.DataCompra,
                NomeSupermercado = compra.NomeSupermercado,
                QuantidadeItens = compra.QuantidadeItens,
                ValorTotal = compra.ValorTotal
            };
        }
    }
}
