using BaseApi.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Domain.Interfaces.Repositorios
{
    public interface IProdutoRepositorio
    {
        Task<Produto?> ObterPorIdAsync(Guid id, CancellationToken ct = default);

        Task<(IEnumerable<Produto> Itens, int Total)> ListarAsync(
            int pagina,
            int tamanhoPagina,
            string? busca,
            CancellationToken ct = default);

        Task AdicionarAsync(Produto produto, CancellationToken ct = default);

        void Atualizar(Produto produto);

        void Remover(Produto produto);

        Task SalvarAsync(CancellationToken ct = default);
    }
}
