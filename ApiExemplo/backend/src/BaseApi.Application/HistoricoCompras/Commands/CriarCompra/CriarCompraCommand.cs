using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Commands.CriarCompra
{
    public class CriarCompraCommand : IRequest<Guid>
    {
        public Guid UsuarioId { get; set; }
        public string NomeSupermercado { get; set; } = string.Empty;

        

        public int QuantidadeItens { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
