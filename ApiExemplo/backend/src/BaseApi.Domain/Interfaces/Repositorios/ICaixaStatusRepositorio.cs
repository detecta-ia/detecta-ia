using BaseApi.Domain.Entidades;


namespace BaseApi.Domain.Repositories;

public interface ICaixaStatusRepositorio
{
    Task<CaixaStatus?> ObterAtualAsync();

    Task CriarAsync(CaixaStatus status);

    Task AtualizarAsync(CaixaStatus status);
}