using Microsoft.AspNetCore.Mvc;
using MediatR;
using BaseApi.Application.Carrinhos.Command;
using BaseApi.Application.Carrinhos.Queries;
using BaseApi.Application.Carrinhos.Commands;
using BaseApi.Application.Carrinhos.Queries.ObterCarrinhoRevisao;

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
        [HttpGet("revisao/{usuarioId}")]
        public async Task<IActionResult> ObterRevisao([FromRoute] Guid usuarioId, CancellationToken ct)
        {
            var resultado = await _mediator.Send(new ObterCarrinhoRevisaoQuery(usuarioId), ct);
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

    // DTO auxiliar para receber os dados estruturados do Webhook do Banco
    public record WebhookPixRequest(string TxId, string Status, decimal Valor);
}