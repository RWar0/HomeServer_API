using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Select;
using HomeServer.Application.Dtos.Vehicles;

namespace HomeServer.Application.Interfaces.Vehicles
{
    public interface IVehiclesService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateVehicleDto createDto);
        public Task<Result<PagedResult<VehicleListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto);
        public Task<Result<VehicleEditDto>> GetForEditAsync(Guid vehicleId);
        public Task<Result<SelectOptionDto[]>> GetForSelectAsync();
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid vehicleId, UpdateVehicleDto updateDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid vehicleId);
    }
}
