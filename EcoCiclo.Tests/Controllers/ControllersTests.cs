using EcoCiclo.API.Controllers;
using EcoCiclo.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Threading.Tasks;
using EcoCiclo.Domain.Entities;
using System.Collections.Generic;

namespace EcoCiclo.Tests.Controllers
{
    public class ControllersTests
    {
        [Fact]
        public async Task Get_Coletas_ReturnsHttpStatusCode200()
        {
            var mockRepository = new Mock<IAgendamentoColetaRepository>();
            mockRepository.Setup(repo => repo.GetPagedAsync(1, 10))
                          .ReturnsAsync(new List<AgendamentoColeta>());

            var controller = new ColetasController(mockRepository.Object);

            var result = await controller.GetPaged(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Get_PontosCriticos_ReturnsHttpStatusCode200()
        {
            var mockRepository = new Mock<IPontoColetaRepository>();
            mockRepository.Setup(repo => repo.GetPontosCriticosAsync())
                          .ReturnsAsync(new List<PontoColeta>());

            var controller = new PontosColetaController(mockRepository.Object);

            var result = await controller.GetCriticos();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}