using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.ParameterChecks;
using HomeServer.Application.Dtos.Select;
using HomeServer.Application.Dtos.WaterChanges;

namespace HomeServer.Application.Interfaces.Aquariums
{
    public interface IAquariumsService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateAquariumDto createDto);
        public Task<Result<AquariumResponseDto[]>> GetAllAsync();
        public Task<Result<SelectOptionDto[]>> GetForSelectAsync();
        public Task<Result<AquariumDetailsDto>> GetDetailsAsync(Guid aquariumId);
        public Task<Result<PagedResult<AquariumPhotosResponseDto>>> GetPhotosAsync(Guid aquariumId, PaginationRequestDto paginationDto);
        public Task<Result<PagedResult<WaterChangeForAquariumListItemDto>>> GetWaterChangesAsync(Guid aquariumId, PaginationRequestDto paginationDto);
        public Task<Result<PagedResult<ParameterCheckForAquariumListItemDto>>> GetParameterChecksAsync(Guid aquariumId, PaginationRequestDto paginationDto);
        public Task<Result<AquariumSummaryDto>> GetForEditAsync(Guid aquariumId);
        public Task<Result<MessageResponseDto>> UploadPhotoAsync(Guid aquariumId, ImageDto fileDto, Guid userId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid aquariumId, UpdateAquariumDto updateDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid aquariumId);
        public Task<Result<MessageResponseDto>> DeletePhotoAsync(Guid aquariumId, Guid photoId);
    }
}
