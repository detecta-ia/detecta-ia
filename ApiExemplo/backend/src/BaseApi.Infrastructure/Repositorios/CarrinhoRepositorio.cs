using BaseApi.Domain.Entidades;
using BaseApi.Domain.Interfaces.Repositorios;
using BaseApi.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Repositorios;

public class CarrinhoRepositorio(AppDbContext contexto) : ICarrinhoRepositorio
{
    public async Task<Carrinho?> ObterAtivoPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
        => await contexto.Set<Carrinho>()
            .Include(c => c.Itens)
                .ThenInclude(i => i.Produto) // Garante o carregamento do produto (ImagemUrl e Tipo)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.Status == "ATIVO", ct);

    public async Task AdicionarAsync(Carrinho carrinho, CancellationToken ct = default)
        => await contexto.Set<Carrinho>().AddAsync(carrinho, ct);

    public void Atualizar(Carrinho carrinho)
        => contexto.Set<Carrinho>().Update(carrinho);

    public async Task SalvarAsync(CancellationToken ct = default)
        => await contexto.SaveChangesAsync(ct);

    public async Task<List<Carrinho>> ListarFinalizadasPorUsuarioIdAsync(Guid usuarioId, CancellationToken ct = default)
        => await contexto.Set<Carrinho>()
            .Include(c => c.Itens)
            .Where(c => c.UsuarioId == usuarioId && c.Status == "FINALIZADO")
            .OrderByDescending(c => c.AtualizadoEm ?? c.CriadoEm)
            .ToListAsync(ct);

    public async Task<List<Carrinho>> ListarFinalizadasPorPeriodoAsync(
        Guid usuarioId,
        DateTime dataInicial,
        DateTime dataFinal,
        CancellationToken ct = default)
        => await contexto.Set<Carrinho>()
            .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
            .Where(c => c.UsuarioId == usuarioId
                     && c.Status == "FINALIZADO"
                     && c.CriadoEm >= dataInicial
                     && c.CriadoEm <= dataFinal)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync(ct);
}