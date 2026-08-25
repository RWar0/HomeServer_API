using HomeServer.Application.Validators;
using HomeServer.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Vehicles
{
    public class CreateVehicleDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Marka pojazdu jest wymagana!")]
        public string Brand { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Model pojazdu jest wymagany!")]
        public string Model { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Rok produkcji pojazdu jest wymagany!")]
        [Range(1800, int.MaxValue, ErrorMessage = "Rok produkcji pojazdu musi większy niż 1800!")]
        [NotFutureIntYear(Name = "Rok produkcji")]
        public int Production { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Typ pojazdu jest wymagany!")]
        public VehiclesEnum Type { get; set; }
    }
}
