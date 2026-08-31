using HomeServer.Application.Dtos.VehicleServiceItems;

namespace HomeServer.Application.Dtos.VehicleServices
{
    public class VehicleServiceForVehicleEditDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public DateOnly Date { get; set; }
        public decimal? Cost { get; set; }

        public List<VehicleServiceItemDto> Items { get; set; } = [];
    }
}
