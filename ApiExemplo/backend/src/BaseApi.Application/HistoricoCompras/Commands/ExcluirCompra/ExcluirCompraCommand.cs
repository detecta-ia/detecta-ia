using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.ExcluirCompra
{
    public class ExcluirCompraCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
