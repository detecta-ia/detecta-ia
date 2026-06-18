using BaseApi.Application.Command.Atualizar_Status;
using BaseApi.Application.Command.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.API.Controllers
{
    [ApiController]
    [Route("api/caixa-status")]
    public class CaixaStatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CaixaStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> ObterStatus()
        {
            var status = await _mediator.Send(
                new BuscarCaixaStatusQuery());

            return Ok(status);
        }

        [HttpPut]
        public async Task<IActionResult> AtualizarStatus(
            [FromBody] AtualizarCaixaStatusCommand command)
        {
            var resultado = await _mediator.Send(command);

            return Ok(resultado);
        }
    }
}
