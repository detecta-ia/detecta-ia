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

    public async Task<Carrinho?> ObterPorIdEUsuarioAsync(Guid id, Guid usuarioId, CancellationToken ct = default)
        => await contexto.Set<Carrinho>()
            .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId, ct);

    public async Task<(IEnumerable<Carrinho> Itens, int Total)> ListarFinalizadasPorUsuarioAsync(
        Guid usuarioId,
        int pagina,
        int tamanhoPagina,
        CancellationToken ct = default)
    {
        var query = contexto.Set<Carrinho>()
            .AsNoTracking()
            .Include(c => c.Itens)
            .Where(c => c.UsuarioId == usuarioId && c.Status == "FINALIZADO");

        int total = await query.CountAsync(ct);

        var itens = await query
            .OrderByDescending(c => c.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(ct);

        return (itens, total);
    }

    public async Task AdicionarAsync(Carrinho carrinho, CancellationToken ct = default)
        => await contexto.Set<Carrinho>().AddAsync(carrinho, ct);

    public void Atualizar(Carrinho carrinho)
        => contexto.Set<Carrinho>().Update(carrinho);

    public async Task SalvarAsync(CancellationToken ct = default)
        => await contexto.SaveChangesAsync(ct);
}