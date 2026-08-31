using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.VehicleServices;

namespace HomeServer.Application.Interfaces.VehicleServices
{
    public interface IVehicleServicesService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateVehicleServiceDto createDto);
        public Task<Result<MessageResponseDto>> CreateForVehicleAsync(Guid vehicleId, CreateVehicleServiceForVehicleDto createDto);
        public Task<Result<PagedResult<VehicleServiceListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto, VehicleServiceFilterDto filterDto);
        public Task<Result<VehicleServiceEditDto>> GetForEditAsync(Guid vehicleServiceId);
        public Task<Result<VehicleServiceForVehicleEditDto>> GetForEditWithoutVehicleAsync(Guid vehicleServiceId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid vehicleServiceId, UpdateVehicleServiceDto updateDto);
        public Task<Result<MessageResponseDto>> UpdateForVehicleAsync(Guid vehicleServiceId, UpdateVehicleServiceForVehicleDto updateDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid vehicleServiceId);
    }
}
