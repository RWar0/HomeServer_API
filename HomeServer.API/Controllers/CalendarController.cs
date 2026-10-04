using HomeServer.API.Common.Extensions;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Calendar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;
        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] DateFilterDto filterDto, CancellationToken ct)
        {
            var result = await _calendarService.GetAllAsync(filterDto, ct);

            return result.ToActionResult(this);
        }
    }
}
