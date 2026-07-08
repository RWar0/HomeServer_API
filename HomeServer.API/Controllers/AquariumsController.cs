using HomeServer.API.Common.Extensions;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Interfaces.Aquariums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AquariumsController : ControllerBase
    {
        private readonly IAquariumsService _aquariumsService;

        public AquariumsController(IAquariumsService aquariumsService)
        {
            _aquariumsService = aquariumsService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<string>> CreateAquarium(AquariumCreateDto createDto)
        {
            var result = await _aquariumsService.Create(createDto);

            return result.ToActionResult(this);
        }

        [HttpGet]
        public async Task<ActionResult<AquariumGetResponseDto[]>> GetAquariums(BasePaginationDto paginationDto)
        {
            var result = await _aquariumsService.GetAll(paginationDto);

            return result.ToActionResult(this);
        }
    }
}
