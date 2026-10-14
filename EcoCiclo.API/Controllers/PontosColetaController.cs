using EcoCiclo.Domain.Entities;
using EcoCiclo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EcoCiclo.API.Controllers
{
    [ApiController]
    [Route("api/pontos-coleta")]
    public class PontosColetaController : ControllerBase
    {
        private readonly IPontoColetaRepository _repository;

        public PontosColetaController(IPontoColetaRepository repository)
        {
            _repository = repository;
        }

        // Endpoint 3: Recebe dados de sensores IoT e gera Alertas Automáticos se lotar
        [HttpPost("alerta")]
        public async Task<IActionResult> RegistrarVolume([FromQuery] int id, [FromQuery] double volumeAtual)
        {
            var ponto = await _repository.GetByIdAsync(id);
            if (ponto == null) return NotFound("Ponto de coleta não encontrado.");

            ponto.CapacidadeAtualKg = volumeAtual;
            ponto.UltimaSincronizacao = DateTime.UtcNow;

            // Alerta automático se atingir o limite
            if (ponto.CapacidadeAtualKg >= ponto.CapacidadeMaximaKg)
            {
                ponto.PrecisaColeta = true;
            }

            await _repository.UpdateAsync(ponto);
            return Ok(new { Mensagem = ponto.PrecisaColeta ? "ALERTA: Capacidade esgotada! Coleta necessária." : "Volume registrado.", Ponto = ponto });
        }

        // Endpoint 4: Retorna pontos críticos para otimização da rota de reciclagem
        [HttpGet("otimizacao")]
        public async Task<IActionResult> GetCriticos()
        {
            var pontosCriticos = await _repository.GetPontosCriticosAsync();
            return Ok(pontosCriticos);
        }

        // Endpoint auxiliar para dados de teste
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PontoColeta ponto)
        {
            ponto.UltimaSincronizacao = DateTime.UtcNow;
            await _repository.AddAsync(ponto);
            return Ok(ponto);
        }
    }
}
