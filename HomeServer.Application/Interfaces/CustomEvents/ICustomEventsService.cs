using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.CalendarEvent;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.CustomEvents;

namespace HomeServer.Application.Interfaces.CustomEvents
{
    public interface ICustomEventsService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateCustomEventDto createDto, Guid createdByUserId);
        public Task<Result<CalendarEventDto[]>> GetAsync(DateFilterDto filterDto);
        public Task<Result<CalendarEventDto>> GetByIdAsync(Guid customEventId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid customEventId, UpdateCustomEventDto updateDto, Guid updatedByUserId);
        public Task<Result<MessageResponseDto>> MarkDoneNowAsync(Guid customEventId, Guid updatedByUserId);
        public Task<Result<MessageResponseDto>> MarkNotDoneAsync(Guid customEventId, Guid updatedByUserId);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid customEventId, Guid deletedByUserId);
    }
}
