using System.Security.Claims;
using BaseApi.Application.Carrinhos.Queries.ObterCarrinhoRevisao;
using BaseApi.Application.Carrinhos.Queries.ObterResumoGastos;
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

    /// <summary>
    /// GET /api/carrinhos/resumo-gastos
    /// [RN01] Calcula automaticamente Total Gasto, Número de Compras, Média por Compra
    /// e Última Compra Realizada a partir das compras (carrinhos finalizados) do usuário.
    /// [RN02] O cálculo é feito em tempo real, então os indicadores já refletem
    /// automaticamente qualquer nova compra concluída.
    /// </summary>
    [HttpGet("resumo-gastos")]
    public async Task<IActionResult> ObterResumoGastos(CancellationToken ct)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
            return BadRequest(new { ok = false, erros = new[] { "Usuário não autenticado." } });

        var resultado = await mediator.Send(new ObterResumoGastosQuery(usuarioId), ct);

        return Ok(new { ok = true, mensagem = "Resumo de gastos carregado.", dados = resultado });
    }
}