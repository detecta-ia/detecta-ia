using BaseApi.Domain.Entidades;
using BaseApi.Domain.Interfaces.Repositorios;
using BaseApi.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Infrastructure.Repositorios
{
    public class ProdutoRepositorio(AppDbContext contexto) : IProdutoRepositorio
    {
        public async Task AdicionarAsync(Produto produto, CancellationToken ct = default)
        { 
            await contexto.Produtos.AddAsync(produto, ct);
        }        

        public void Atualizar(Produto produto)
        {
            contexto.Produtos.Update(produto);
        }

        public async Task<(IEnumerable<Produto> Itens, int Total)> ListarAsync(
     int pagina,
     int tamanhoPagina,
     string? busca,
     CancellationToken ct = default)
        {
            // 1. Criamos a query base sem rastreamento para otimizar leitura
            var query = contexto.Produtos.AsNoTracking();

            // 2. Se o usuário enviou um termo de busca, filtramos os resultados
            if (!string.IsNullOrWhiteSpace(busca))
            {
                var buscaLower = busca.ToLower();
                query = query.Where(p => p.Nome.ToLower().Contains(buscaLower) ||
                                         p.Categoria.ToLower().Contains(buscaLower));
            }

            // 3. Obtemos o total de registros que batem com o filtro
            int total = await query.CountAsync(ct);

            // 4. Executamos a paginação eficiente direto no banco de dados
            var itens = await query
                .OrderBy(p => p.Nome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync(ct);

            // 5. Retornamos a tupla esperada pela interface e pelo Handler
            return (itens, total);
        }

        public async Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        {
            return await contexto.Produtos
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public void Remover(Produto produto)
        {
            contexto.Produtos.Remove(produto);
        }

        public async Task SalvarAsync(CancellationToken ct = default)
        {
            await contexto.SaveChangesAsync(ct);
        }
    }
}
