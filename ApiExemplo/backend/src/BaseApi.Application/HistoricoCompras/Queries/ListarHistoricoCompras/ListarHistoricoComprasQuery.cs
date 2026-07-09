using BaseApi.Application.HistoricoCompras.Queries.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Queries.ListarHistoricoCompras
{
    public class ListarHistoricoComprasQuery
     : IRequest<List<HistoricoCompraDto>>
    {
        public Guid UsuarioId { get; set; }
    }
}
