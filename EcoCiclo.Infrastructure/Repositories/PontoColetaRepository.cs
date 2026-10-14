using EcoCiclo.Domain.Entities;
using EcoCiclo.Domain.Interfaces;
using EcoCiclo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcoCiclo.Infrastructure.Repositories
{
    public class PontoColetaRepository : IPontoColetaRepository
    {
        private readonly AppDbContext _context;

        public PontoColetaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PontoColeta?> GetByIdAsync(int id) =>
            await _context.PontosColeta.FindAsync(id);

        public async Task<IEnumerable<PontoColeta>> GetPontosCriticosAsync() =>
            await _context.PontosColeta.Where(p => p.PrecisaColeta || p.CapacidadeAtualKg >= p.CapacidadeMaximaKg).ToListAsync();

        public async Task AddAsync(PontoColeta ponto)
        {
            await _context.PontosColeta.AddAsync(ponto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PontoColeta ponto)
        {
            _context.PontosColeta.Update(ponto);
            await _context.SaveChangesAsync();
        }
    }
}
