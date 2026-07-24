using BaseApi.Domain.Excecoes;
using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;

namespace BaseApi.Application.Carrinhos.Commands.FinalizarCompra;

public class FinalizarCompraHandler(ICarrinhoRepositorio carrinhoRepositorio)
    : IRequestHandler<FinalizarCompraCommand, FinalizarCompraResposta>
{
    public async Task<FinalizarCompraResposta> Handle(FinalizarCompraCommand request, CancellationToken ct)
    {
        var carrinho = await carrinhoRepositorio.ObterAtivoPorUsuarioIdAsync(request.UsuarioId, ct)
            ?? throw new ExcecaoNaoEncontrado("Nenhum carrinho ativo encontrado para finalizar a compra.");

        if (carrinho.Itens.Count == 0)
            throw new ExcecaoDominio("Não é possível finalizar uma compra sem itens.");

        carrinho.Status = "FINALIZADO";
        carrinho.AtualizadoEm = DateTime.UtcNow;

        carrinhoRepositorio.Atualizar(carrinho);
        await carrinhoRepositorio.SalvarAsync(ct);

        var valorTotal = carrinho.Itens.Sum(i => (decimal)(i.Quantidade * i.PrecoUnitarioCents) / 100);

        return new FinalizarCompraResposta(carrinho.Id, carrinho.AtualizadoEm.Value, valorTotal);
    }
}