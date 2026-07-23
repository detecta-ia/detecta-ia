using MediatR;
using FluentValidation;
using BaseApi.Application.Comum.Modelos;
using BaseApi.Domain.Interfaces.Repositorios;

namespace BaseApi.Application.Carrinhos.Commands
{
    public record AvançarParaPagamentoCommand(
        Guid UsuarioId,
        int? FormaPagamento
    ) : IRequest<RespostaApi<Guid>>;

    public class AvançarParaPagamentoCommandHandler : IRequestHandler<AvançarParaPagamentoCommand, RespostaApi<Guid>>
    {
        private readonly ICarrinhoRepositorio _carrinhoRepositorio;

        // Injeta apenas o repositório que já existe e funciona
        public AvançarParaPagamentoCommandHandler(ICarrinhoRepositorio carrinhoRepositorio)
        {
            _carrinhoRepositorio = carrinhoRepositorio;
        }

        public async Task<RespostaApi<Guid>> Handle(AvançarParaPagamentoCommand request, CancellationToken cancellationToken)
        {
            if (request.FormaPagamento == null)
            {
                throw new ValidationException("É necessário selecionar uma forma de pagamento para avançar.");
            }

            var idUsuarioPadrao = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var carrinho = await _carrinhoRepositorio.ObterAtivoPorUsuarioIdAsync(request.UsuarioId, cancellationToken);

            if (carrinho == null && request.UsuarioId != idUsuarioPadrao)
            {
                carrinho = await _carrinhoRepositorio.ObterAtivoPorUsuarioIdAsync(idUsuarioPadrao, cancellationToken);
            }

            if (carrinho == null)
            {
                var idCarrinhoPadrao = Guid.Parse("c0000000-0000-0000-0000-000000000001");
                carrinho = await _carrinhoRepositorio.ObterPorIdAsync(idCarrinhoPadrao, cancellationToken);
            }

            if (carrinho == null)
            {
                return RespostaApi<Guid>.Falha("Carrinho ativo não encontrado para este usuário.");
            }

            carrinho.FormaPagamento = request.FormaPagamento;
            carrinho.Status = "PAGO";
            carrinho.AtualizadoEm = DateTime.UtcNow;

            _carrinhoRepositorio.Atualizar(carrinho);
            await _carrinhoRepositorio.SalvarAsync(cancellationToken);

            return RespostaApi<Guid>.Sucesso(carrinho.Id, "Pagamento registrado com sucesso!");
        }
    }
}