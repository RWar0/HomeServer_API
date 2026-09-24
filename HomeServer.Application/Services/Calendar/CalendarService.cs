using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Calendar;
using HomeServer.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Calendar
{
    public class CalendarService : BaseService, ICalendarService
    {
        private const int PREV_DAYS_TO_FETCH = 7;
        private const int NEXT_DAYS_TO_FETCH = 7;

        private readonly IEnumerable<ICalendarEventProvider> _calendarEventProviders;

        public CalendarService(MyDbContext context, ILogger<CalendarService> logger, IEnumerable<ICalendarEventProvider> calendarEventProviders) : base(context, logger)
        {
            _calendarEventProviders = calendarEventProviders;
        }

        public async Task<Result<CalendarEventDto[]>> GetAllAsync(DateFilterDto filterDto, CancellationToken ct)
        {
            try
            {
                var effectiveFilterDto = CreateEffectiveFilter(filterDto);

                var events = new List<CalendarEventDto>();

                foreach (var provider in _calendarEventProviders)
                {
                    var providerEvents = await provider.GetCalendarEventsAsync(effectiveFilterDto, ct);

                    events.AddRange(providerEvents);
                }

                return Result.Success(events.ToArray());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get calendar events!");
                return Result.Failure("Nie można pobrać wydarzeń kalendarza!", ProblemType.InternalServerError);
            }
        }

        // Helpers
        private static DateFilterDto CreateEffectiveFilter(DateFilterDto filterDto)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var fromDate = (filterDto.FromDate ??
                    new DateOnly(today.Year, today.Month, 1)
                ).AddDays(-PREV_DAYS_TO_FETCH);

            var toDate = (filterDto.ToDate ??
                    fromDate.AddMonths(1).AddDays(-1)
                ).AddDays(NEXT_DAYS_TO_FETCH);

            return new DateFilterDto
            {
                FromDate = fromDate,
                ToDate = toDate
            };
        }
    }
}
