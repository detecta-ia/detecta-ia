using BaseApi.Domain.Entidades;

namespace BaseApi.Domain.Interfaces.Repositorios;

public interface ICarrinhoRepositorio
{
    Task<Carrinho?> ObterAtivoPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
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
}