using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.WaterChanges;

namespace HomeServer.Application.Interfaces.WaterChanges
{
    public interface IWaterChangesService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateWaterChangeDto createDto);
        public Task<Result<MessageResponseDto>> CreateForAquariumAsync(Guid aquariumId, CreateWaterChangeForAquariumDto createDto);
        public Task<Result<PagedResult<WaterChangeListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto, WaterChangeFilterDto filterDto);
        public Task<Result<WaterChangeEditDto>> GetForEditAsync(Guid waterChangeId);
        public Task<Result<WaterChangeEditForAquariumDto>> GetForAquariumEditAsync(Guid waterChangeId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid waterChangeId, UpdateWaterChangeDto updateDto);
        public Task<Result<MessageResponseDto>> UpdateForAquariumAsync(Guid waterChangeId, UpdateWaterChangeForAquariumDto updateDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid waterChangeId);
    }
}
