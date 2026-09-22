using HomeServer.Domain.Base;
using HomeServer.Domain.Enums;

namespace HomeServer.Domain.Models
{
    public class CustomEvent : BaseModel
    {
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        public string? Location { get; set; }

        public CustomEventCategoryEnum? Category { get; set; }

        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }

        public DateOnly? CompletionDate { get; set; } = null;

        public Guid CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; } = default!;
    }
}
