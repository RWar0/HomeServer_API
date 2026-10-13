using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Calculators;
using HomeServer.Application.Interfaces.Calculators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CalculatorsController : ControllerBase
    {
        private readonly ICalculatorsService _calculatorsService;

        public CalculatorsController(ICalculatorsService calculatorsService)
        {
            _calculatorsService = calculatorsService;
        }

        [HttpPost("co2")]
        public async Task<IActionResult> CalculateCo2(CalculatorCo2RequestDto request)
        {
            var result = await _calculatorsService.GetCalculatedCo2Async(request);

            return result.ToActionResult(this);
        }
    }
}
