using System;

namespace EcoCiclo.Domain.Entities
{
    public class AgendamentoColeta
    {
        public int Id { get; set; }
        public string EmpresaUsuario { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public double PesoEstimadoKg { get; set; }
        public DateTime DataAgendamento { get; set; }
        public string Status { get; set; } = "Pendente";
    }
}
