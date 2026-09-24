using HomeServer.API.Common.Extensions;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
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
        public async Task<ActionResult<MessageResponseDto>> CreateWaterChange(CreateWaterChangeDto createDto)
        {
            var result = await _waterChangesService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }
        
        [HttpPost("for-aquarium/{aquariumId}")]
        public async Task<ActionResult<MessageResponseDto>> CreateWaterChangeForAquarium([FromRoute] Guid aquariumId, CreateWaterChangeForAquariumDto createDto)
        {
            var result = await _waterChangesService.CreateForAquariumAsync(aquariumId, createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<ActionResult<PagedResult<WaterChangeListItemDto>>> GetList([FromQuery] PaginationRequestDto paginationDto, [FromQuery] WaterChangeFilterDto filterDto)
        {
            var result = await _waterChangesService.GetAllAsync(paginationDto, filterDto);

            return result.ToActionResult(this);
        }
        
        [HttpGet("{waterChangeId}")]
        public async Task<ActionResult<WaterChangeDto>> GetById([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.GetByIdAsync(waterChangeId);

            return result.ToActionResult(this);
        }
        
        [HttpGet("{waterChangeId}/for-edit")]
        public async Task<ActionResult<WaterChangeEditDto>> GetForEdit([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.GetForEditAsync(waterChangeId);

            return result.ToActionResult(this);
        }
        
        [HttpGet("{waterChangeId}/for-aquarium-edit")]
        public async Task<ActionResult<WaterChangeEditForAquariumDto>> GetForAquariumEdit([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.GetForAquariumEditAsync(waterChangeId);

            return result.ToActionResult(this);
        }

        [HttpPut("{waterChangeId}")]
        public async Task<ActionResult<MessageResponseDto>> UpdateWaterChange([FromRoute] Guid waterChangeId, UpdateWaterChangeDto updateDto)
        {
            var result = await _waterChangesService.UpdateAsync(waterChangeId, updateDto);

            return result.ToActionResult(this);
        }
        [HttpPut("for-aquarium/{waterChangeId}")]
        public async Task<ActionResult<MessageResponseDto>> UpdateWaterChangeOfAquarium([FromRoute] Guid waterChangeId, UpdateWaterChangeForAquariumDto updateDto)
        {
            var result = await _waterChangesService.UpdateForAquariumAsync(waterChangeId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{waterChangeId}")]
        public async Task<ActionResult<MessageResponseDto>> Delete([FromRoute] Guid waterChangeId)
        {
            var result = await _waterChangesService.DeleteAsync(waterChangeId);

            return result.ToActionResult(this);
        }
    }
}
