using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Calculators
{
    public class CalculatorCo2RequestDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "KH jest wymagane!")]
        [Range(0, 30, ErrorMessage = "KH musi być w przedziale 0-30!")]
        public double Kh { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "PH jest wymagane!")]
        [Range(0, 14, ErrorMessage = "PH musi być w przedziale 0-14!")]
        public double Ph { get; set; }
    }
}
