using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.HistoricoCompras.Queries.DTOs
{
    public class HistoricoCompraDto
    {
        public Guid Id { get; set; }

        public DateTime DataCompra { get; set; }

        public string NomeSupermercado { get; set; } = string.Empty;

        public int QuantidadeItens { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
