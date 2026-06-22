using System.Security.Claims;
using BaseApi.Application.Carrinhos.Queries.ObterCarrinhoRevisao;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarrinhosController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// GET /api/carrinhos/revisao
    /// Retorna a lista completa para revisão agrupada por tipo com subtotais e imagens.
    /// </summary>
    [HttpGet("revisao")]
    public async Task<IActionResult> ObterRevisao(CancellationToken ct)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
            return BadRequest(new { ok = false, erros = new[] { "Usuário não autenticado." } });

        var resultado = await mediator.Send(new ObterCarrinhoRevisaoQuery(usuarioId), ct);

        return Ok(new { ok = true, mensagem = "Dados do carrinho carregados.", dados = resultado });
    }
}