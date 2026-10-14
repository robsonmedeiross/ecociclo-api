using EcoCiclo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcoCiclo.Domain.Interfaces
{
    public interface IAgendamentoColetaRepository
    {
        Task<IEnumerable<AgendamentoColeta>> GetPagedAsync(int page, int pageSize);
        Task AddAsync(AgendamentoColeta agendamento);
    }
}
