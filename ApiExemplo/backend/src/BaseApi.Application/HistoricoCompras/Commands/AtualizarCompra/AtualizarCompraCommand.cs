using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.AtualizarCompra
{
    public class AtualizarCompraCommand : IRequest
    {
        public Guid Id { get; set; }

        public string NomeSupermercado { get; set; } = string.Empty;

        public DateTime DataCompra { get; set; }

        public int QuantidadeItens { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
