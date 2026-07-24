using BaseApi.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Domain.Interfaces.Repositorios
{
    public interface IHistoricoCompraRepositorio
    {
        Task AdicionarAsync(HistoricoCompra compra);

        Task<HistoricoCompra?> ObterPorIdAsync(Guid id);

        Task<IEnumerable<HistoricoCompra>> ObterPorUsuarioAsync(Guid usuarioId);

        Task AtualizarAsync(HistoricoCompra compra);

        Task RemoverAsync(Guid id);
    }
}
