using HomeServer.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.CustomEvents
{
    public class CreateCustomEventDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa wydarzenia jest wymagana!")]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }
        public string? Location { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Kategoria wydarzenia jest wymagana!")]
        public CalendarEventCategoryEnum? Category { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data wykonania jest wymagana!")]
        public DateOnly Date { get; set; }
        public TimeOnly? Time { get; set; }
    }
}
