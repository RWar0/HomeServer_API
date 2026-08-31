using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.VehicleServiceItems
{
    public class UpdateVehicleServiceItemDto
    {
        public Guid? Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Nazwa pozycji jest wymagana!")]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Koszt pozycji musi być dodatni!")]
        public decimal? Cost { get; set; }
    }
}
