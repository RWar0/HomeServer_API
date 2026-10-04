using HomeServer.Application.Dtos.VehicleServiceItems;
using HomeServer.Application.Validators;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.VehicleServices
{
    public class UpdateVehicleServiceDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa serwisu jest wymagana!")]
        public string Title { get; set; } = default!;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Data serwisu jest wymagana!")]
        [NotFutureDate(Name = "Data serwisu")]
        public DateOnly Date { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Przebieg przy serwisie musi być dodatni!")]
        public int? Mileage { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Koszt serwisu musi być dodatni!")]
        public decimal? Cost { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pojazd jest wymagany!")]
        public Guid VehicleId { get; set; }

        public List<UpdateVehicleServiceItemDto> Items { get; set; } = [];
    }
}
