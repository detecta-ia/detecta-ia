using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Domain.Entidades
{
    public class HistoricoCompra
    {
        public Guid Id { get; private set; }

        public Guid UsuarioId { get; private set; }

        public string NomeSupermercado { get; private set; } = string.Empty;

        public DateTime DataCompra { get; private set; }

        public int QuantidadeItens { get; private set; }

        public decimal ValorTotal { get; private set; }

        protected HistoricoCompra()
        {
        }

        public HistoricoCompra(
            Guid usuarioId,
            string nomeSupermercado,
            int quantidadeItens,
            decimal valorTotal)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            NomeSupermercado = nomeSupermercado;
            QuantidadeItens = quantidadeItens;
            ValorTotal = valorTotal;
            DataCompra = DateTime.UtcNow;
        }

        public void Atualizar(
            string nomeSupermercado,
            DateTime dataCompra,
            int quantidadeItens,
            decimal valorTotal)
        {
            NomeSupermercado = nomeSupermercado;
            DataCompra = dataCompra;
            QuantidadeItens = quantidadeItens;
            ValorTotal = valorTotal;
        }




    }
}
