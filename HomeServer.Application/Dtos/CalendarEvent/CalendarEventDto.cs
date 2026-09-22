namespace HomeServer.Application.Dtos.CalendarEvent
{
    public class CalendarEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;

        public string? Description { get; set; }
        public string? Location { get; set; }

        public string? Category { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }

        public DateOnly? CompletionDate { get; set; }

        public bool IsCustomEvent { get; set; } = false;
    }
}
