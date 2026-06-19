using BaseApi.Domain.Entidades;

namespace BaseApi.Domain.Interfaces.Repositorios;

public interface ICarrinhoRepositorio
{
    Task<Carrinho?> ObterAtivoPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default);
    Task AdicionarAsync(Carrinho carrinho, CancellationToken ct = default);
    void Atualizar(Carrinho carrinho);
    Task SalvarAsync(CancellationToken ct = default);
}