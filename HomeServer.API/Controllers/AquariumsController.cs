using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AquariumsController : ControllerBase
    {
        private readonly IAquariumsService _aquariumsService;

        public AquariumsController(IAquariumsService aquariumsService)
        {
            _aquariumsService = aquariumsService;
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<ActionResult<MessageResponseDto>> CreateAquarium(AquariumCreateDto createDto)
        {
            var result = await _aquariumsService.Create(createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<ActionResult<AquariumGetResponseDto[]>> GetAquariums()
        {
            var result = await _aquariumsService.GetAll();

            return result.ToActionResult(this);
        }

        [HttpGet("for-edit/{aquariumId}")]
        public async Task<ActionResult<AquariumGetResponseDto[]>> GetAquariumForEdit([FromRoute] Guid aquariumId)
        {
            var result = await _aquariumsService.GetForEdit(aquariumId);

            return result.ToActionResult(this);
        }

        [HttpPut("update/{aquariumId}")]
        public async Task<ActionResult<AquariumGetResponseDto[]>> Update([FromRoute] Guid aquariumId, AquariumCreateDto editDto)
        {
            var result = await _aquariumsService.Update(aquariumId, editDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{aquariumId}")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<ActionResult<MessageResponseDto>> Delete([FromRoute] Guid aquariumId)
        {
            var result = await _aquariumsService.Delete(aquariumId);

            return result.ToActionResult(this);
        }
    }
}
