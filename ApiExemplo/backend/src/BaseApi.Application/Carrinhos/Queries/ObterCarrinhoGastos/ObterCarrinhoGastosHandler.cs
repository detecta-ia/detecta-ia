using BaseApi.Domain.Interfaces.Repositorios;
using MediatR;

namespace BaseApi.Application.Carrinhos.Queries.ObterResumoGastos;

public class ObterResumoGastosHandler(ICarrinhoRepositorio carrinhoRepositorio)
    : IRequestHandler<ObterResumoGastosQuery, ResumoGastosDto>
{
    public async Task<ResumoGastosDto> Handle(ObterResumoGastosQuery request, CancellationToken cancellationToken)
    {
        var compras = await carrinhoRepositorio.ListarFinalizadasPorUsuarioIdAsync(request.UsuarioId, cancellationToken);

        // Nenhuma compra concluída ainda → resumo zerado
        if (compras.Count == 0)
            return new ResumoGastosDto(0m, 0, 0m, null);

        // [RN01] Total Gasto — soma de (quantidade x preço unitário) de todos os itens de todas as compras
        int totalGastoCents = compras.Sum(carrinho => carrinho.Itens.Sum(item => item.Quantidade * item.PrecoUnitarioCents));
        decimal totalGasto = totalGastoCents / 100m;

        // Número de Compras — quantidade de carrinhos finalizados
        int numeroDeCompras = compras.Count;

        // Média de Gasto por Compra
        decimal mediaGastoPorCompra = Math.Round(totalGasto / numeroDeCompras, 2);

        // Última Compra Realizada — data mais recente entre as compras finalizadas
        DateTime ultimaCompraRealizada = compras.Max(carrinho => carrinho.AtualizadoEm ?? carrinho.CriadoEm);

        return new ResumoGastosDto(
            Math.Round(totalGasto, 2),
            numeroDeCompras,
            mediaGastoPorCompra,
            ultimaCompraRealizada
        );
    }
}
