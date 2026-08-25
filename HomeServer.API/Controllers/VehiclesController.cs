using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Vehicles;
using HomeServer.Application.Interfaces.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiclesService _vehiclesService;

        public VehiclesController(IVehiclesService vehiclesService)
        {
            _vehiclesService = vehiclesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle(CreateVehicleDto createDto)
        {
            var result = await _vehiclesService.CreateAsync(createDto);

            return result.ToActionResult(this);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequestDto paginationDto)
        {
            var result = await _vehiclesService.GetAllAsync(paginationDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{vehicleId}/for-edit")]
        public async Task<IActionResult> GetById([FromRoute] Guid vehicleId)
        {
            var result = await _vehiclesService.GetForEditAsync(vehicleId);

            return result.ToActionResult(this);
        }

        [HttpPut("{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle([FromRoute] Guid vehicleId, UpdateVehicleDto updateDto)
        {
            var result = await _vehiclesService.UpdateAsync(vehicleId, updateDto);

            return result.ToActionResult(this);
        }

        [HttpDelete("{vehicleId}")]
        public async Task<IActionResult> DeleteVehicle([FromRoute] Guid vehicleId)
        {
            var result = await _vehiclesService.DeleteAsync(vehicleId);

            return result.ToActionResult(this);
        }
    }
}
