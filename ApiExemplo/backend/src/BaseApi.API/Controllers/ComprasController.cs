using System.Security.Claims;
using BaseApi.Application.Comum.Modelos;
using BaseApi.Application.Compras.Queries.ListarHistoricoCompras;
using BaseApi.Application.Compras.Queries.ObterDetalhesCompra;
using BaseApi.Domain.Excecoes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseApi.API.Controllers;

/// <summary>
/// Endpoints de consulta ao histórico de compras do cliente.
/// Uma "compra" corresponde a um Carrinho com Status = "FINALIZADO".
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ComprasController(IMediator mediator) : ControllerBase
{
    // =========================================================
    // GET /api/compras?pagina=1&tamanhoPagina=10
    // =========================================================
    /// <summary>
    /// Lista o histórico de compras do usuário autenticado, para que ele
    /// possa selecionar qualquer uma delas e ver os detalhes. [RN01]
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(RespostaApi<ResultadoPaginado<CompraResumoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarHistorico(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken ct = default)
    {
        var usuarioId = ObterUsuarioIdAutenticado();

        var resultado = await mediator.Send(new ListarHistoricoComprasQuery(usuarioId, pagina, tamanhoPagina), ct);

        return Ok(RespostaApi<ResultadoPaginado<CompraResumoDto>>.Sucesso(resultado));
    }

    // =========================================================
    // GET /api/compras/{id}
    // =========================================================
    /// <summary>
    /// Retorna os detalhes completos de uma compra específica: produtos,
    /// quantidade, preço unitário, subtotal por item e valor total. [RN02]
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespostaApi<CompraDetalheDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterDetalhes(Guid id, CancellationToken ct)
    {
        var usuarioId = ObterUsuarioIdAutenticado();

        var resultado = await mediator.Send(new ObterDetalhesCompraQuery(id, usuarioId), ct);

        return Ok(RespostaApi<CompraDetalheDto>.Sucesso(resultado));
    }

    private Guid ObterUsuarioIdAutenticado()
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
            throw new ExcecaoNaoAutorizado("Usuário não autenticado.");

        return usuarioId;
    }
}
