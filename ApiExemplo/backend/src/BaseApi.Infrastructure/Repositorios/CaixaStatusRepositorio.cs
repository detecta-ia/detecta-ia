using BaseApi.Domain.Entidades;
using BaseApi.Domain.Repositories;
using BaseApi.Infrastructure.Dados;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApi.Infrastructure.Repositorios
{
    public class CaixaStatusRepositorio : ICaixaStatusRepositorio
    {
        private readonly AppDbContext _context;

        public CaixaStatusRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CaixaStatus?> ObterAtualAsync()
        {
            return await _context.CaixaStatus
                .OrderByDescending(x => x.DataAtualizacao)
                .FirstOrDefaultAsync();
        }

        public async Task CriarAsync(CaixaStatus status)
        {
            await _context.CaixaStatus.AddAsync(status);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(CaixaStatus status)
        {
            _context.CaixaStatus.Update(status);
            await _context.SaveChangesAsync();
        }
    }
}
