using HomeServer.Application.Dtos.VehicleServiceItems;

namespace HomeServer.Application.Dtos.VehicleServices
{
    public class VehicleServiceDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public DateOnly Date { get; set; }
        public decimal? Cost { get; set; }
        public int? Mileage { get; set; }

        public string VehicleName { get; set; } = default!;

        public List<VehicleServiceItemDto> Items { get; set; } = [];
    }
}
