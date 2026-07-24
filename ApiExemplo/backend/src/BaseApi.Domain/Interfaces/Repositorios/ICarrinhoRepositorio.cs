using BaseApi.Domain.Entidades;

namespace BaseApi.Domain.Interfaces.Repositorios;

public interface ICarrinhoRepositorio
{
    Task<Carrinho?> ObterAtivoPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);

    /// <summary>
    /// Busca uma compra (carrinho) específica pelo Id, garantindo que pertence ao usuário informado.
    /// Usado na tela de "detalhes da compra". [RN01]
    /// </summary>
    Task<Carrinho?> ObterPorIdEUsuarioAsync(Guid id, Guid usuarioId, CancellationToken ct = default);

    /// <summary>
    /// Lista o histórico de compras finalizadas (Status = "FINALIZADO") de um usuário, paginado.
    /// </summary>
    Task<(IEnumerable<Carrinho> Itens, int Total)> ListarFinalizadasPorUsuarioAsync(
        Guid usuarioId,
        int pagina,
        int tamanhoPagina,
        CancellationToken ct = default);

    Task AdicionarAsync(Carrinho carrinho, CancellationToken ct = default);
    void Atualizar(Carrinho carrinho);
    Task SalvarAsync(CancellationToken ct = default);
    Task<List<Carrinho>> ListarFinalizadasPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);

    /// <summary>
    /// [RN01][RN02] Lista as compras (carrinhos com Status = "FINALIZADO") do usuário
    /// cuja data de criação esteja dentro do período informado (inclusive nas duas pontas).
    /// </summary>
    Task<List<Carrinho>> ListarFinalizadasPorPeriodoAsync(
        Guid usuarioId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken ct = default);
    Task<Carrinho?> ObterPorIdAsync(Guid carrinhoId, CancellationToken cancellationToken = default);
}