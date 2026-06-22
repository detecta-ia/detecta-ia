using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Produtos.Commands.ExcluirProduto;

public record ExcluirProdutoCommand(Guid Id) : IRequest<Unit>;