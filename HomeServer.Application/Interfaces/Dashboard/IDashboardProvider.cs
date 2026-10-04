using HomeServer.Application.Dtos.Dashboar;

namespace HomeServer.Application.Interfaces.Dashboard
{
    public interface IDashboardProvider
    {
        public Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken ct);
        public Task<IEnumerable<DashboardAttentionItemDto>> GetDashboardAttentionItemsAsync(int limit, CancellationToken ct);
        public Task<IEnumerable<DashboardUpcomingEventDto>> GetDashboardUpcomingEventsAsync(int limit, CancellationToken ct);
        public Task<IEnumerable<DashboardRecentActivityDto>> GetDashboardRecentActivitiesAsync(int limit, CancellationToken ct);
    }
}
