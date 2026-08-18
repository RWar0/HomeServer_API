using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.ParameterChecks
{
    public class CreateParameterCheckDto
    {
        [Range(0, 14, ErrorMessage = "PH musi być w zakresie 0-14")]
        public double? Ph { get; set; }

        [Range(0, 30, ErrorMessage = "KH musi być w zakresie 0-30")]
        public double? Kh { get; set; }

        [Range(0, 50, ErrorMessage = "GH musi być w zakresie 0-50")]
        public double? Gh { get; set; }

        [Range(0, 1000, ErrorMessage = "NO3 musi być w zakresie 0-1000")]
        public double? No3 { get; set; }

        [Range(0, 100, ErrorMessage = "NO2 musi być w zakresie 0-100")]
        public double? No2 { get; set; }

        [Range(0, 100, ErrorMessage = "Temperatura musi być w zakresie 0-100")]
        public double? Temperature { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data pomiaru jest wymagana!")]
        [NotFutureDate(Name = "Data pomiaru")]
        public DateOnly MeasuredAt { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Akwarium jest wymagane!")]
        public Guid AquariumId { get; set; }
    }
}
