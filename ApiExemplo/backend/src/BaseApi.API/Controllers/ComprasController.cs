using BaseApi.Application.HistoricoCompras.Commands.AtualizarCompra;
using BaseApi.Application.HistoricoCompras.Commands.CriarCompra;
using BaseApi.Application.HistoricoCompras.Commands.ExcluirCompra;
using BaseApi.Application.HistoricoCompras.Queries.BuscarCompraPorId;
using BaseApi.Application.HistoricoCompras.Queries.ListarHistoricoCompras;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/compras")]
[Authorize]
public class ComprasController : ControllerBase
{
    private readonly IMediator _mediator;

    public ComprasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        return Ok(await _mediator.Send(new ListarHistoricoComprasQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Buscar(Guid id)
    {
        return Ok(await _mediator.Send(
            new BuscarCompraPorIdQuery
            {
                Id = id
            }));
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        CriarCompraCommand command)
    {
        var id = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(Buscar),
            new { id },
            id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        AtualizarCompraCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _mediator.Send(
            new ExcluirCompraCommand
            {
                Id = id
            });

        return NoContent();
    }
}