using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.AtualizarProduto
{
    public class AtualizarProdutoCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal Preco { get; set; }
    }
}
