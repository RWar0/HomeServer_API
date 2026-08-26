using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.VehicleFuelings;

namespace HomeServer.Application.Interfaces.VehicleFuelings
{
    public interface IVehicleFuelingsService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateVehicleFuelingDto createDto);
        public Task<Result<MessageResponseDto>> CreateForVehicleAsync(Guid vehicleId, CreateVehicleFuelingForVehicleDto createDto);
        public Task<Result<PagedResult<VehicleFuelingListItem>>> GetAllAsync(PaginationRequestDto paginationDto, VehicleFuelingFilterDto filterDto);
        public Task<Result<VehicleFuelingEditDto>> GetForEditAsync(Guid vehicleFuelingId);
        public Task<Result<VehicleFuelingWithoutVehicleEditDto>> GetForEditWithoutVehicleAsync(Guid vehicleFuelingId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid vehicleFuelingId, UpdateVehicleFuelingDto updateDto);
        public Task<Result<MessageResponseDto>> UpdateForVehicleAsync(Guid vehicleFuelingId, UpdateVehicleFuelingForVehicleDto updateDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid vehicleFuelingId);
    }
}