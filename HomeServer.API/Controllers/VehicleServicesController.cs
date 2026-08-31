using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.VehicleServices;
using HomeServer.Application.Interfaces.VehicleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/vehicle-services")]
    [ApiController]
    [Authorize]
    public class VehicleServicesController : ControllerBase
    {
        private readonly IVehicleServicesService _vehicleServicesService;

        public VehicleServicesController(IVehicleServicesService vehicleServicesService)
        {
            _vehicleServicesService = vehicleServicesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicleService(CreateVehicleServiceDto createDto)
        {
            var result = await _vehicleServicesService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }

        [HttpPost("for-vehicle/{vehicleId}")]
        public async Task<IActionResult> CreateVehicleServiceForVehicle(Guid vehicleId, CreateVehicleServiceForVehicleDto createDto)
        {
            var result = await _vehicleServicesService.CreateForVehicleAsync(vehicleId, createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequestDto paginationDto, [FromQuery] VehicleServiceFilterDto filterDto)
        {
            var result = await _vehicleServicesService.GetAllAsync(paginationDto, filterDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{vehicleServiceId}/for-edit")]
        public async Task<IActionResult> GetById([FromRoute] Guid vehicleServiceId)
        {
            var result = await _vehicleServicesService.GetForEditAsync(vehicleServiceId);

            return result.ToActionResult(this);
        }

        [HttpGet("{vehicleServiceId}/for-edit-without-vehicle")]
        public async Task<IActionResult> GetByIdWithoutVehicle([FromRoute] Guid vehicleServiceId)
        {
            var result = await _vehicleServicesService.GetForEditWithoutVehicleAsync(vehicleServiceId);

            return result.ToActionResult(this);
        }

        [HttpPut("{vehicleServiceId}")]
        public async Task<IActionResult> UpdateVehicleService([FromRoute] Guid vehicleServiceId, UpdateVehicleServiceDto updateDto)
        {
            var result = await _vehicleServicesService.UpdateAsync(vehicleServiceId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpPut("{vehicleServiceId}/without-vehicle")]
        public async Task<IActionResult> UpdateVehicleServiceForVehicle([FromRoute] Guid vehicleServiceId, UpdateVehicleServiceForVehicleDto updateDto)
        {
            var result = await _vehicleServicesService.UpdateForVehicleAsync(vehicleServiceId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{vehicleServiceId}")]
        public async Task<IActionResult> DeleteVehicleService([FromRoute] Guid vehicleServiceId)
        {
            var result = await _vehicleServicesService.DeleteAsync(vehicleServiceId);

            return result.ToActionResult(this);
        }
    }
}
