using HomeServer.API.Common.Extensions;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Dtos.ParameterChecks;
using HomeServer.Application.Interfaces.ParameterChecks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/parameter-checks")]
    [ApiController]
    [Authorize]
    public class ParameterChecksController : ControllerBase
    {
        private readonly IParametersCheckService _parametersCheckService;
        public ParameterChecksController(IParametersCheckService parametersCheckService)
        {
            _parametersCheckService = parametersCheckService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateParameterChecks(ParameterCheckCreateEditDto createDto)
        {
            var parameterChecks = await _parametersCheckService.Create(createDto);

            return parameterChecks.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetParameterChecksList([FromQuery] PaginationDto paginationDto, [FromQuery] ParameterChecksListFiltrationDto filtrationDto)
        {
            var parameterChecks = await _parametersCheckService.GetAll(paginationDto, filtrationDto);

            return parameterChecks.ToActionResult(this);
        }

        [HttpGet("{parameterCheckId}/for-edit")]
        public async Task<IActionResult> GetParameterChecksForEdit([FromRoute] Guid parameterCheckId)
        {
            var parameterChecks = await _parametersCheckService.GetForEdit(parameterCheckId);

            return parameterChecks.ToActionResult(this);
        }

        [HttpPut("{parameterCheckId}")]
        public async Task<IActionResult> UpdateParameterChecks([FromRoute] Guid parameterCheckId, ParameterCheckCreateEditDto editDto)
        {
            var parameterChecks = await _parametersCheckService.Update(parameterCheckId, editDto);

            return parameterChecks.ToActionResult(this);
        }

        [HttpDelete("{parameterCheckId}")]
        public async Task<IActionResult> DeleteParameterChecks([FromRoute] Guid parameterCheckId)
        {
            var parameterChecks = await _parametersCheckService.Delete(parameterCheckId);

            return parameterChecks.ToActionResult(this);
        }
    }
}
