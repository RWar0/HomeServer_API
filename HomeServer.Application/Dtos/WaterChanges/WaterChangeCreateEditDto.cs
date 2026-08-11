using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangeCreateEditDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ilość podmienionej wody jest wymagana!")]
        [Range(1, 10000, ErrorMessage = "Ilość podmienionej wody musi być dodatnia i nie większa niż 10000l!")]
        public decimal Amount { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data podmiany jest wymagana!")]
        [NotFutureDate(Name = "Data podmiany")]
        public DateOnly ChangeDate { get; set; }
    }
}
