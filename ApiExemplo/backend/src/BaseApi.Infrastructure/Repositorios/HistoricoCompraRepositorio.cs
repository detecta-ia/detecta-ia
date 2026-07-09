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
    public class HistoricoCompraRepository
     : IHistoricoCompraRepositorio
    {
        private readonly AppDbContext _context;

        public HistoricoCompraRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(HistoricoCompra compra)
        {
            await _context.HistoricoCompras.AddAsync(compra);

            await _context.SaveChangesAsync();
        }

        public async Task<HistoricoCompra?> ObterPorIdAsync(Guid id)
        {
            return await _context.HistoricoCompras
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<HistoricoCompra>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.HistoricoCompras
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.DataCompra)
                .ToListAsync();
        }

        public async Task AtualizarAsync(HistoricoCompra compra)
        {
            _context.HistoricoCompras.Update(compra);

            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var compra = await ObterPorIdAsync(id);

            if (compra is null)
                return;

            _context.HistoricoCompras.Remove(compra);

            await _context.SaveChangesAsync();
        }
    }
}
