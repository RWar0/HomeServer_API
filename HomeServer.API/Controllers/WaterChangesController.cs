using HomeServer.API.Common.Extensions;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.WaterChanges;
using HomeServer.Application.Interfaces.WaterChanges;
using HomeServer.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/water-changes")]
    [ApiController]
    [Authorize]
    public class WaterChangesController : ControllerBase
    {
        private readonly IWaterChangesService _waterChangesService;

        public WaterChangesController(IWaterChangesService waterChangesService)
        {
            _waterChangesService = waterChangesService;
        }

        [HttpPost]
        public async Task<ActionResult<MessageResponseDto>> CreateWaterChange(WaterChangeCreateEditDto createDto)
        {
            var result = await _waterChangesService.CreateWaterChange(createDto);

            return result.ToActionResult(this);
        }
        
        [HttpPost("for-aquarium/{aquariumId}")]
        public async Task<ActionResult<MessageResponseDto>> CreateWaterChangeForAquarium([FromRoute] Guid aquariumId, WaterChangeCreateEditForAquariumDto createDto)
        {
            var result = await _waterChangesService.CreateWaterChangeForAquarium(aquariumId, createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<ActionResult<PagedResult<WaterChangesGetResponseDto>>> GetList([FromQuery] PaginationDto paginationDto)
        {
            var result = await _waterChangesService.GetAll(paginationDto);

            return result.ToActionResult(this);
        }
        
        [HttpGet("{waterChangeId}/for-edit")]
        public async Task<ActionResult<WaterChangeForEditResponseDto>> GetForEdit([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.GetForEdit(waterChangeId);

            return result.ToActionResult(this);
        }
        
        [HttpGet("{waterChangeId}/for-aquarium-edit")]
        public async Task<ActionResult<WaterChangeForAquariumEditResponseDto>> GetForAquariumEdit([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.GetForEdit(waterChangeId);

            return result.ToActionResult(this);
        }

        [HttpPut("{waterChangeId}")]
        public async Task<ActionResult<MessageResponseDto>> UpdateWaterChange([FromRoute] Guid waterChangeId, WaterChangeCreateEditDto editDto)
        {
            var result = await _waterChangesService.UpdateWaterChange(waterChangeId, editDto);

            return result.ToActionResult(this);
        }
        [HttpPut("for-aquarium/{waterChangeId}")]
        public async Task<ActionResult<MessageResponseDto>> UpdateWaterChangeOfAquarium([FromRoute] Guid waterChangeId, WaterChangeCreateEditForAquariumDto editDto)
        {
            var result = await _waterChangesService.UpdateWaterChangeOfAquarium(waterChangeId, editDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{waterChangeId}")]
        public async Task<ActionResult<MessageResponseDto>> Delete([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.Delete(waterChangeId);

            return result.ToActionResult(this);
        }
    }
}
