using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.VehicleFuelings
{
    public class CreateVehicleFuelingForVehicleDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Ilość paliwa jest wymagana!")]
        [Range(0, 500, ErrorMessage = "Ilość paliwa musi być dodatnia i mniejsza od 500!")]
        public double Quantity { get; set; }

        [Range(0, 5000, ErrorMessage = "Kwota musi być dodatnia i mniejsza od 5000!")]
        public decimal? Cost { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data tankowania jest wymagana!")]
        [NotFutureDate(Name = "Data tankowania")]
        public DateOnly Date { get; set; }
    }
}
