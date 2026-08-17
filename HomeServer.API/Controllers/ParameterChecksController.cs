using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Pagination;
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
        private readonly IParameterChecksService _parametersCheckService;
        public ParameterChecksController(IParameterChecksService parametersCheckService)
        {
            _parametersCheckService = parametersCheckService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateParameterChecks(CreateParameterCheckDto createDto)
        {
            var parameterChecks = await _parametersCheckService.CreateAsync(createDto);

            return parameterChecks.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetParameterChecksList([FromQuery] PaginationRequestDto paginationDto, [FromQuery] ParameterCheckFilterDto filtrationDto)
        {
            var parameterChecks = await _parametersCheckService.GetAllAsync(paginationDto, filtrationDto);

            return parameterChecks.ToActionResult(this);
        }

        [HttpGet("{parameterCheckId}/for-edit")]
        public async Task<IActionResult> GetParameterChecksForEdit([FromRoute] Guid parameterCheckId)
        {
            var parameterChecks = await _parametersCheckService.GetForEditAsync(parameterCheckId);

            return parameterChecks.ToActionResult(this);
        }

        [HttpPut("{parameterCheckId}")]
        public async Task<IActionResult> UpdateParameterChecks([FromRoute] Guid parameterCheckId, UpdateParameterCheckDto updateDto)
        {
            var parameterChecks = await _parametersCheckService.UpdateAsync(parameterCheckId, updateDto);

            return parameterChecks.ToActionResult(this);
        }

        [HttpDelete("{parameterCheckId}")]
        public async Task<IActionResult> DeleteParameterChecks([FromRoute] Guid parameterCheckId)
        {
            var parameterChecks = await _parametersCheckService.DeleteAsync(parameterCheckId);

            return parameterChecks.ToActionResult(this);
        }
    }
}
