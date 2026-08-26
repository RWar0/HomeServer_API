using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.VehicleFuelings;
using HomeServer.Application.Interfaces.VehicleFuelings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/vehicle-fuelings")]
    [ApiController]
    [Authorize]
    public class VehicleFuelingsController : ControllerBase
    {
        private readonly IVehicleFuelingsService _vehicleFuelingsService;

        public VehicleFuelingsController(IVehicleFuelingsService vehicleFuelingsService)
        {
            _vehicleFuelingsService = vehicleFuelingsService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicleFueling(CreateVehicleFuelingDto createDto)
        {
            var result = await _vehicleFuelingsService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }

        [HttpPost("for-vehicle/{vehicleId}")]
        public async Task<IActionResult> CreateVehicleFuelingForVehicle(Guid vehicleId, CreateVehicleFuelingForVehicleDto createDto)
        {
            var result = await _vehicleFuelingsService.CreateForVehicleAsync(vehicleId, createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequestDto paginationDto, [FromQuery] VehicleFuelingFilterDto filterDto)
        {
            var result = await _vehicleFuelingsService.GetAllAsync(paginationDto, filterDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{vehicleFuelingId}/for-edit")]
        public async Task<IActionResult> GetById([FromRoute] Guid vehicleFuelingId)
        {
            var result = await _vehicleFuelingsService.GetForEditAsync(vehicleFuelingId);

            return result.ToActionResult(this);
        }

        [HttpGet("{vehicleFuelingId}/for-edit-without-vehicle")]
        public async Task<IActionResult> GetByIdWithoutVehicle([FromRoute] Guid vehicleFuelingId)
        {
            var result = await _vehicleFuelingsService.GetForEditWithoutVehicleAsync(vehicleFuelingId);

            return result.ToActionResult(this);
        }

        [HttpPut("{vehicleFuelingId}")]
        public async Task<IActionResult> UpdateVehicleFueling([FromRoute] Guid vehicleFuelingId, UpdateVehicleFuelingDto updateDto)
        {
            var result = await _vehicleFuelingsService.UpdateAsync(vehicleFuelingId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpPut("{vehicleFuelingId}/without-vehicle")]
        public async Task<IActionResult> UpdateVehicleFuelingForVehicle([FromRoute] Guid vehicleFuelingId, UpdateVehicleFuelingForVehicleDto updateDto)
        {
            var result = await _vehicleFuelingsService.UpdateForVehicleAsync(vehicleFuelingId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{vehicleFuelingId}")]
        public async Task<IActionResult> DeleteVehicleFueling([FromRoute] Guid vehicleFuelingId)
        {
            var result = await _vehicleFuelingsService.DeleteAsync(vehicleFuelingId);

            return result.ToActionResult(this);
        }
    }
}
