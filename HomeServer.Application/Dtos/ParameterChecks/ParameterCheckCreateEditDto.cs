using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.ParameterChecks
{
    public class ParameterCheckCreateEditDto
    {
        public double? Ph { get; set; }
        public double? Kh { get; set; }
        public double? Gh { get; set; }
        public double? No3 { get; set; }
        public double? No2 { get; set; }
        public double? Temperature { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data pomiaru jest wymagana!")]
        [NotFutureDate(Name = "Data pomiaru")]
        public DateOnly MeasuredAt { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Akwarium jest wymagane!")]
        public Guid AquariumId { get; set; }
    }
}
