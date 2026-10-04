using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;

namespace HomeServer.Application.Interfaces.Calendar
{
    public interface ICalendarEventProvider
    {
        public Task<IEnumerable<CalendarEventDto>> GetCalendarEventsAsync(DateFilterDto filterDto, CancellationToken ct);
    }
}
