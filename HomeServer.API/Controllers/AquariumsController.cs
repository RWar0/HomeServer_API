using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Select;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Application.Interfaces.UserSession;
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
        private readonly ICurrentUserService _currentUserService;

        public AquariumsController(IAquariumsService aquariumsService, ICurrentUserService currentUserService)
        {
            _aquariumsService = aquariumsService;
            _currentUserService = currentUserService;
        }

        [HttpPost("create")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<ActionResult<MessageResponseDto>> CreateAquarium(CreateAquariumDto createDto)
        {
            var result = await _aquariumsService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<ActionResult<AquariumResponseDto[]>> GetAll()
        {
            var result = await _aquariumsService.GetAllAsync();

            return result.ToActionResult(this);
        }
        
        [HttpGet("for-select")]
        public async Task<ActionResult<SelectOptionDto[]>> GetForSelect()
        {
            var result = await _aquariumsService.GetForSelectAsync();

            return result.ToActionResult(this);
        }
        
        [HttpGet("{aquariumId}/details")]
        public async Task<ActionResult<AquariumResponseDto[]>> GetAquariumDetails([FromRoute] Guid aquariumId)
        {
            var result = await _aquariumsService.GetDetailsAsync(aquariumId);

            return result.ToActionResult(this);
        }
        
        [HttpGet("{aquariumId}/photos")]
        public async Task<ActionResult<AquariumResponseDto[]>> GetAquariumPhotos([FromRoute] Guid aquariumId, [FromQuery] PaginationRequestDto paginationDto)
        {
            var result = await _aquariumsService.GetPhotosAsync(aquariumId, paginationDto);

            return result.ToActionResult(this);
        }
        [HttpGet("{aquariumId}/water-changes")]
        public async Task<ActionResult<AquariumResponseDto[]>> GetAquariumWaterChanges([FromRoute] Guid aquariumId, [FromQuery] PaginationRequestDto paginationDto)
        {
            var result = await _aquariumsService.GetWaterChangesAsync(aquariumId, paginationDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{aquariumId}/for-edit")]
        public async Task<ActionResult<AquariumResponseDto[]>> GetAquariumForEdit([FromRoute] Guid aquariumId)
        {
            var result = await _aquariumsService.GetForEditAsync(aquariumId);

            return result.ToActionResult(this);
        }

        [HttpPost("{aquariumId}/upload-photo")]
        public async Task<ActionResult<MessageResponseDto>> UploadPhoto([FromRoute] Guid aquariumId, [FromForm] ImageDto fileDto)
        {
            var result = await _aquariumsService.UploadPhotoAsync(aquariumId, fileDto, _currentUserService.UserId);

            return result.ToActionResult(this);
        }

        [HttpPut("{aquariumId}")]
        public async Task<ActionResult<AquariumResponseDto[]>> Update([FromRoute] Guid aquariumId, UpdateAquariumDto updateDto)
        {
            var result = await _aquariumsService.UpdateAsync(aquariumId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{aquariumId}")]
        [Authorize(Roles = nameof(RolesEnum.Admin))]
        public async Task<ActionResult<MessageResponseDto>> Delete([FromRoute] Guid aquariumId)
        {
            var result = await _aquariumsService.DeleteAsync(aquariumId);

            return result.ToActionResult(this);
        }

        [HttpDelete("{aquariumId}/photo/{photoId}")]
        [Authorize(Roles = $"{nameof(RolesEnum.Admin)},{nameof(RolesEnum.Moderator)}")]
        public async Task<ActionResult<MessageResponseDto>> DeletePhoto([FromRoute] Guid aquariumId, [FromRoute] Guid photoId)
        {
            var result = await _aquariumsService.DeletePhotoAsync(aquariumId, photoId);

            return result.ToActionResult(this);
        }
    }
}
