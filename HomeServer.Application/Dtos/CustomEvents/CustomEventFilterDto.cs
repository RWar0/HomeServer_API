using HomeServer.Domain.Enums;

namespace HomeServer.Application.Dtos.CustomEvents
{
    public class CustomEventFilterDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public CalendarEventCategoryEnum? Category { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }

        public bool? IsCompleted { get; set; }
        public DateOnly? FromCompletionDate { get; set; }
        public DateOnly? ToCompletionDate { get; set; }
    }
}
