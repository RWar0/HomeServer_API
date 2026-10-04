using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Dashboar;
using HomeServer.Application.Interfaces.Dashboard;
using HomeServer.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Dashboard
{
    public class DashboardService : BaseService, IDashboardService
    {
        private readonly IEnumerable<IDashboardProvider> _dashboardProviders;

        public DashboardService(MyDbContext context, ILogger<DashboardService> logger, IEnumerable<IDashboardProvider> dashboardProviders) : base(context, logger)
        {
            _dashboardProviders = dashboardProviders;
        }

        public async Task<Result<DashboardSummaryDto>> GetDashboardSummaryAsync(CancellationToken ct)
        {
            try
            {
                var summary = new DashboardSummaryDto
                {
                    TodayEventsCount = 0,
                    OverdueEventsCount = 0,
                    RemainingDaysToNextEvent = null,
                    RemainingEventTitle = null,
                    TodayCompletedEventsCount = 0
                };

                foreach (var provider in _dashboardProviders)
                {
                    var providerSummary = await provider.GetDashboardSummaryAsync(ct);
                    summary.TodayEventsCount += providerSummary.TodayEventsCount;
                    summary.OverdueEventsCount += providerSummary.OverdueEventsCount;
                    summary.TodayCompletedEventsCount += providerSummary.TodayCompletedEventsCount;

                    if (providerSummary.RemainingDaysToNextEvent.HasValue)
                    {
                        if (!summary.RemainingDaysToNextEvent.HasValue || providerSummary.RemainingDaysToNextEvent < summary.RemainingDaysToNextEvent)
                        {
                            summary.RemainingDaysToNextEvent = providerSummary.RemainingDaysToNextEvent;
                            summary.RemainingEventTitle = providerSummary.RemainingEventTitle;
                        }
                    }
                }

                return Result.Success(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get dashboard summary!");
                return Result.Failure("Nie można pobrać podsumowania dashboardu!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<IEnumerable<DashboardAttentionItemDto>>> GetDashboardAttentionItemsAsync(int limit, CancellationToken ct)
        {
            try
            {
                var attentionItems = new List<DashboardAttentionItemDto>();

                foreach (var provider in _dashboardProviders)
                {
                    var providerAttentionItems = await provider.GetDashboardAttentionItemsAsync(limit, ct);
                    attentionItems.AddRange(providerAttentionItems);
                }

                var orderedAttentionItems = attentionItems
                                                .OrderBy(item => item.Date)
                                                .Take(limit)
                                                .ToList();

                return Result<IEnumerable<DashboardAttentionItemDto>>.Success(orderedAttentionItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get calendar events!");
                return Result.Failure("Nie można pobrać wydarzeń kalendarza!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<IEnumerable<DashboardRecentActivityDto>>> GetDashboardRecentActivitiesAsync(int limit, CancellationToken ct)
        {
            try
            {
                var recentActivities = new List<DashboardRecentActivityDto>();

                foreach (var provider in _dashboardProviders)
                {
                    var providerRecentActivities = await provider.GetDashboardRecentActivitiesAsync(limit, ct);
                    recentActivities.AddRange(providerRecentActivities);
                }

                var orderedRecentActivities = recentActivities
                                                .OrderByDescending(item => item.Date)
                                                .Take(limit)
                                                .ToList();

                return Result<IEnumerable<DashboardRecentActivityDto>>.Success(orderedRecentActivities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get recent activities!");
                return Result.Failure("Nie można pobrać ostatnich działań!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<IEnumerable<DashboardUpcomingEventDto>>> GetDashboardUpcomingEventsAsync(int limit, CancellationToken ct)
        {
            try
            {
                var upcomingEvents = new List<DashboardUpcomingEventDto>();

                foreach (var provider in _dashboardProviders)
                {
                    var providerUpcomingEvents = await provider.GetDashboardUpcomingEventsAsync(limit, ct);
                    upcomingEvents.AddRange(providerUpcomingEvents);
                }

                var orderedUpcomingEvents = upcomingEvents
                                                .OrderBy(item => item.Date)
                                                .Take(limit)
                                                .ToList();

                return Result<IEnumerable<DashboardUpcomingEventDto>>.Success(orderedUpcomingEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get upcoming events!");
                return Result.Failure("Nie można pobrać nadchodzących wydarzeń!", ProblemType.InternalServerError);
            }
        }
    }
}
