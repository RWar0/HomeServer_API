using HomeServer.Domain.Enums;

namespace HomeServer.Application.Dtos.CustomEvents
{
    public class CustomEventListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;

        public string? Description { get; set; }
        public string? Location { get; set; }

        public string? Category { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }

        public DateOnly? CompletionDate { get; set; }
    }
}
