using EcoCiclo.Domain.Entities;
using EcoCiclo.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EcoCiclo.API.Controllers
{
    [ApiController]
    [Route("api/coletas")]
    public class ColetasController : ControllerBase
    {
        private readonly IAgendamentoColetaRepository _repository;

        public ColetasController(IAgendamentoColetaRepository repository)
        {
            _repository = repository;
        }

        // Endpoint 1: Listagem Paginada Otimizada
        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0) return BadRequest("Página ou tamanho inválidos.");

            var dados = await _repository.GetPagedAsync(page, pageSize);
            return Ok(dados);
        }

        // Endpoint 2: Cadastro Crítico Protegido por JWT com Validação
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] AgendamentoColeta model)
        {
            if (model.PesoEstimadoKg <= 0)
                return BadRequest("O peso estimado do resíduo deve ser maior que zero.");

            if (string.IsNullOrEmpty(model.EmpresaUsuario))
                return BadRequest("O nome da empresa ou usuário é obrigatório.");

            model.DataAgendamento = DateTime.UtcNow;
            await _repository.AddAsync(model);

            return Ok(model);
        }
    }
}