using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;

namespace HomeServer.Application.Interfaces.Calendar
{
    public interface ICalendarService
    {
        public Task<Result<CalendarEventDto[]>> GetAllAsync(DateFilterDto filterDto, CancellationToken ct);
    }
}
