using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.VehicleFuelings
{
    public class CreateVehicleFuelingDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Iloœæ paliwa jest wymagana!")]
        [Range(0, 500, ErrorMessage = "Iloœæ paliwa musi byæ dodatnia i mniejsza od 500!")]
        public double Quantity { get; set; }

        [Range(0, 5000, ErrorMessage = "Kwota musi byæ dodatnia i mniejsza od 5000!")]
        public decimal? Cost { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data tankowania jest wymagana!")]
        [NotFutureDate(Name = "Data tankowania")]
        public DateOnly Date { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pojazd jest wymagany!")]
        public Guid VehicleId { get; set; }
    }
}