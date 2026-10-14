using EcoCiclo.Domain.Entities;
using EcoCiclo.Domain.Interfaces;
using EcoCiclo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcoCiclo.Infrastructure.Repositories
{
    public class AgendamentoColetaRepository : IAgendamentoColetaRepository
    {
        private readonly AppDbContext _context;

        public AgendamentoColetaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AgendamentoColeta>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.AgendamentosColeta
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddAsync(AgendamentoColeta agendamento)
        {
            await _context.AgendamentosColeta.AddAsync(agendamento);
            await _context.SaveChangesAsync();
        }
    }
}