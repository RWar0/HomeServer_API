using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Common.Select;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.WaterChanges;

namespace HomeServer.Application.Interfaces.Aquariums
{
    public interface IAquariumsService
    {
        public Task<Result<MessageResponseDto>> Create(AquariumCreateDto createDto);
        public Task<Result<AquariumGetResponseDto[]>> GetAll();
        public Task<Result<SelectResponseDto[]>> GetForSelect();
        public Task<Result<AquariumDetailsDto>> GetDetails(Guid aquariumId);
        public Task<Result<PagedResult<AquariumPhotosResponseDto>>> GetPhotos(Guid aquariumId, PaginationDto paginationDto);
        public Task<Result<PagedResult<WaterChangesOfAquariumGetResponseDto>>> GetWaterChanges(Guid aquariumId, PaginationDto paginationDto);
        public Task<Result<AquariumBaseDataDto>> GetForEdit(Guid aquariumId);
        public Task<Result<MessageResponseDto>> UploadPhoto(Guid aquariumId, ImageDto fileDto, Guid userId);
        public Task<Result<MessageResponseDto>> Update(Guid aquariumId, AquariumCreateDto editDto);
        public Task<Result<MessageResponseDto>> Delete(Guid aquariumId);
        public Task<Result<MessageResponseDto>> DeletePhoto(Guid aquariumId, Guid photoId);
    }
}
