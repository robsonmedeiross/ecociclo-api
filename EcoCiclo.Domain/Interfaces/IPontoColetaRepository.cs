using EcoCiclo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcoCiclo.Domain.Interfaces
{
    public interface IPontoColetaRepository
    {
        Task<PontoColeta?> GetByIdAsync(int id);
        Task<IEnumerable<PontoColeta>> GetPontosCriticosAsync();
        Task AddAsync(PontoColeta ponto);
        Task UpdateAsync(PontoColeta ponto);
    }
}