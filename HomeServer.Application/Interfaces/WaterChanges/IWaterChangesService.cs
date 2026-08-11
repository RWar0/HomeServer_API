using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.WaterChanges;

namespace HomeServer.Application.Interfaces.WaterChanges
{
    public interface IWaterChangesService
    {
        public Task<Result<MessageResponseDto>> CreateWaterChangeForAquarium(Guid aquariumId, WaterChangeCreateEditDto createDto);
        public Task<Result<PagedResult<WaterChangesGetResponseDto>>> GetAll(PaginationDto paginationDto);
        public Task<Result<WaterChangeForEditResponseDto>> GetForEdit(Guid waterChangeId);
        public Task<Result<MessageResponseDto>> UpdateWaterChangeOfAquarium(Guid waterChangeId, WaterChangeCreateEditDto editDto);
        public Task<Result<MessageResponseDto>> Delete(Guid waterChangeId);
    }
}
