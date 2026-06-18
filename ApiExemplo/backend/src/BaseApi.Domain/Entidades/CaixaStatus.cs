using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Domain.Entidades
{

    public class CaixaStatus
    {
        public Guid Id { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public DateTime DataAtualizacao { get; private set; }

        protected CaixaStatus() { }

        public CaixaStatus(string status)
        {
            Id = Guid.NewGuid();
            Status = status;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void AtualizarStatus(string status)
        {
            Status = status;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}