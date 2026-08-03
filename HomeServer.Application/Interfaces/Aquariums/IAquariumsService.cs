using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;

namespace HomeServer.Application.Interfaces.Aquariums
{
    public interface IAquariumsService
    {
        public Task<Result<MessageResponseDto>> Create(AquariumCreateDto createDto);
        public Task<Result<AquariumGetResponseDto[]>> GetAll();
        public Task<Result<AquariumBaseDataDto>> GetForEdit(Guid aquariumId);
        public Task<Result<MessageResponseDto>> UploadPhoto(Guid aquariumId, ImageDto fileDto, Guid userId);
        public Task<Result<MessageResponseDto>> Update(Guid aquariumId, AquariumCreateDto editDto);
        public Task<Result<MessageResponseDto>> Delete(Guid aquariumId);
    }
}
