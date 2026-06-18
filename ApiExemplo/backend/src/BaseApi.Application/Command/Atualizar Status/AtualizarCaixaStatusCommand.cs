using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Application.Command.Atualizar_Status
{
    
    
        public record AtualizarCaixaStatusCommand(string Status)
      : IRequest<bool>;
        
}
