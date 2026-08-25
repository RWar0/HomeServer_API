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
        public async Task<IActionResult> CreateVehicle(CreateVehicleFuelingDto createDto)
        {
            var result = await _vehicleFuelingsService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequestDto paginationDto, [FromQuery] VehicleFuelingFilterDto filterDto)
        {
            var result = await _vehicleFuelingsService.GetAllAsync(paginationDto, filterDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{vehicleId}/for-edit")]
        public async Task<IActionResult> GetById([FromRoute] Guid vehicleId)
        {
            var result = await _vehicleFuelingsService.GetForEditAsync(vehicleId);

            return result.ToActionResult(this);
        }

        [HttpPut("{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle([FromRoute] Guid vehicleId, UpdateVehicleFuelingDto updateDto)
        {
            var result = await _vehicleFuelingsService.UpdateAsync(vehicleId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] Guid vehicleId)
        {
            var result = await _vehicleFuelingsService.DeleteAsync(vehicleId);

            return result.ToActionResult(this);
        }
    }
}
