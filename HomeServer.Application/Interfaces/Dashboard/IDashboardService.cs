using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Dashboar;

namespace HomeServer.Application.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        public Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(CancellationToken ct);
        public Task<Result<IEnumerable<DashboardAttentionItemDto>>> GetDashboardAttentionItemsAsync(int limit, CancellationToken ct);
        public Task<Result<IEnumerable<DashboardUpcomingEventDto>>> GetDashboardUpcomingEventsAsync(int limit, CancellationToken ct);
        public Task<Result<IEnumerable<DashboardRecentActivityDto>>> GetDashboardRecentActivitiesAsync(int limit, CancellationToken ct);
    }
}
