using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.VehicleServiceItems
{
    public class CreateVehicleServiceItemDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa pozycji jest wymagana!")]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Koszt pozycji musi być dodatni!")]
        public decimal? Cost { get; set; }
    }
}
