using HomeServer.API.Common.Extensions;
using HomeServer.Application.Interfaces.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private const int ATTENTION_ITEM_LIMIT = 5;
        private const int UPCOMING_EVENTS_LIMIT = 5;
        private const int RECENT_ACTIVITIES_LIMIT = 4;

        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary(CancellationToken ct)
        {
            var result = await _dashboardService.GetDashboardSummaryAsync(ct);

            return result.ToActionResult(this);
        }

        [HttpGet("attention-items")]
        public async Task<IActionResult> GetDashboardAttentionItems(CancellationToken ct)
        {
            var result = await _dashboardService.GetDashboardAttentionItemsAsync(ATTENTION_ITEM_LIMIT, ct);

            return result.ToActionResult(this);
        }

        [HttpGet("upcoming-events")]
        public async Task<IActionResult> GetDashboardUpcomingEvents(CancellationToken ct)
        {
            var result = await _dashboardService.GetDashboardUpcomingEventsAsync(UPCOMING_EVENTS_LIMIT, ct);

            return result.ToActionResult(this);
        }

        [HttpGet("recent-activities")]
        public async Task<IActionResult> GetDashboardRecentActivities(CancellationToken ct)
        {
            var result = await _dashboardService.GetDashboardRecentActivitiesAsync(RECENT_ACTIVITIES_LIMIT, ct);

            return result.ToActionResult(this);
        }
    }
}
