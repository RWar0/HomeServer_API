using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.CustomEvents;
using HomeServer.Application.Interfaces.CustomEvents;
using HomeServer.Application.Interfaces.UserSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/custom-events")]
    [ApiController]
    [Authorize]
    public class CustomEventsController : ControllerBase
    {
        private readonly ICustomEventsService _customEventsService;
        private readonly ICurrentUserService _currentUserService;

        public CustomEventsController(ICustomEventsService customEventsService, ICurrentUserService currentUserService)
        {
            _customEventsService = customEventsService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateCustomEventDto createDto)
        {
            var result = await _customEventsService.CreateAsync(createDto, _currentUserService.UserId);

            return result.ToActionResult(this);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] DateFilterDto filterDto)
        {
            var result = await _customEventsService.GetAsync(filterDto);

            return result.ToActionResult(this);
        }

        [HttpGet("{customEventId}")]
        public async Task<IActionResult> GetByIdAsync(Guid customEventId)
        {
            var result = await _customEventsService.GetByIdAsync(customEventId);

            return result.ToActionResult(this);
        }

        [HttpPut("{customEventId}")]
        public async Task<IActionResult> UpdateAsync(Guid customEventId, UpdateCustomEventDto updateDto)
        {
            var result = await _customEventsService.UpdateAsync(customEventId, updateDto, _currentUserService.UserId);

            return result.ToActionResult(this);
        }

        [HttpPatch("{customEventId}/mark-done")]
        public async Task<IActionResult> MarkDoneNowAsync(Guid customEventId)
        {
            var result = await _customEventsService.MarkDoneNowAsync(customEventId, _currentUserService.UserId);

            return result.ToActionResult(this);
        }

        [HttpDelete("{customEventId}")]
        public async Task<IActionResult> DeleteAsync(Guid customEventId)
        {
            var result = await _customEventsService.DeleteAsync(customEventId, _currentUserService.UserId);

            return result.ToActionResult(this);
        }
    }
}
