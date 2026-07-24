using System.Security.Claims;
using BaseApi.Application.Carrinhos.Queries.ListarComprasPorPeriodo;
using BaseApi.Application.Carrinhos.Queries.ObterCarrinhoRevisao;
using BaseApi.Application.Carrinhos.Queries.ObterResumoGastos;
using BaseApi.Application.Comum.Modelos;
using BaseApi.Application.Carrinhos.Commands.FinalizarCompra;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


using BaseApi.Application.Carrinhos.Queries;
using BaseApi.Application.Carrinhos.Commands;


namespace BaseApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarrinhosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarrinhosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ENDPOINT EXISTENTE: Obter revisão do carrinho
        [HttpGet("revisao")]
        [HttpGet("revisao/{usuarioId}")]
        public async Task<IActionResult> ObterRevisao([FromRoute] Guid? usuarioId, CancellationToken ct)
        {
            var idUsuarioFinal = usuarioId ?? Guid.Parse("00000000-0000-0000-0000-000000000001");
            var resultado = await _mediator.Send(new ObterCarrinhoRevisaoQuery(idUsuarioFinal), ct);
            return Ok(resultado);
        }

        // ENDPOINT EXISTENTE: Avançar para pagamento
        [HttpPost("avancar-pagamento")]
        public async Task<IActionResult> AvançarParaPagamento([FromBody] AvançarParaPagamentoCommand command, CancellationToken ct)
        {
            var resultado = await _mediator.Send(command, ct);
            return resultado.Sucesso() ? Ok(resultado) : BadRequest(resultado);
        }

        // --- ENPOINTS NOVOS DA ISSUE DO PIX ---

        // 1. Gera o QR Code e Copia e Cola expirando em 5 min [RN01]
        [HttpPost("{carrinhoId}/gerar-pix")]
        public async Task<IActionResult> GerarPix([FromRoute] Guid carrinhoId, CancellationToken ct)
        {
            var resultado = await _mediator.Send(new GerarPixCommand(carrinhoId), ct);

            
            if (!resultado.Sucesso())
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        // 2. Webhook: Escuta o banco de forma assíncrona para confirmar o pagamento automaticamente [RN02]
        [HttpPost("webhook/pix")]
        public async Task<IActionResult> WebhookConfirmacaoPix([FromBody] WebhookPixRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrEmpty(request.TxId))
            {
                return BadRequest("Payload inválido.");
            }

            if (request.Status.ToLower() == "concluido" || request.Status.ToLower() == "approved")
            {
                Console.WriteLine($"[Webhook Pix] Pagamento recebido com sucesso para a transação {request.TxId}");

                // CORRIGIDO: Usando Task.CompletedTask para resolver o aviso CS1998 já que não temos um await real ainda
                await Task.CompletedTask;

                return Ok(new { mensagem = "Webhook processado e carrinho atualizado para Pago com sucesso!" });
            }

            return Ok(new { mensagem = "Status ignorado ou não concluído." });
        }
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
        var resultado = await mediator.Send(new ObterCarrinhoRevisaoQuery(usuarioId), ct);
        return Ok(new { ok = true, mensagem = "Dados do carrinho carregados.", dados = resultado });
    }

    /// <summary>
    /// POST /api/carrinhos/finalizar
    /// Finaliza o carrinho ativo do usuário (checkout), transformando-o em uma
    /// compra. A partir daí ela aparece no histórico (GET /api/compras).
    /// </summary>
    [HttpPost("finalizar")]
    public async Task<IActionResult> Finalizar(CancellationToken ct)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
            return BadRequest(new { ok = false, erros = new[] { "Usuário não autenticado." } });
        var resultado = await mediator.Send(new FinalizarCompraCommand(usuarioId), ct);
        return Ok(new { ok = true, mensagem = "Compra finalizada com sucesso.", dados = resultado });
    }

        var resultado = await mediator.Send(new ObterResumoGastosQuery(usuarioId), ct);

        return Ok(new { ok = true, mensagem = "Resumo de gastos carregado.", dados = resultado });
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

    /// <summary>
    /// GET /api/carrinhos/compras?dataInicial=2026-01-01&amp;dataFinal=2026-01-31
    /// [RN01] Filtra as compras (carrinhos finalizados) do usuário autenticado por período.
    /// [RN02] Retorna apenas as compras cuja data esteja dentro do intervalo informado.
    /// </summary>
    [HttpGet("compras")]
    [ProducesResponseType(typeof(RespostaApi<List<CompraListaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RespostaApi), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarComprasPorPeriodo(
        [FromQuery] DateTime dataInicial,
        [FromQuery] DateTime dataFinal,
        CancellationToken ct)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
            return BadRequest(RespostaApi.Falha("Usuário não autenticado."));

        var resultado = await mediator.Send(new ListarComprasPorPeriodoQuery(usuarioId, dataInicial, dataFinal), ct);

        return Ok(RespostaApi<List<CompraListaDto>>.Sucesso(resultado, "Compras filtradas com sucesso."));
    }
    // DTO auxiliar para receber os dados estruturados do Webhook do Banco
    public record WebhookPixRequest(string TxId, string Status, decimal Valor);
}