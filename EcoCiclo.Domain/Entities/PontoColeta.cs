using System;

namespace EcoCiclo.Domain.Entities
{
    public class PontoColeta
    {
        public int Id { get; set; }
        public string Localizacao { get; set; } = string.Empty;
        public string TipoResiduo { get; set; } = string.Empty;
        public double CapacidadeMaximaKg { get; set; }
        public double CapacidadeAtualKg { get; set; }
        public bool PrecisaColeta { get; set; }
        public DateTime UltimaSincronizacao { get; set; }
    }
}
