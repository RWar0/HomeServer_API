using HomeServer.Domain.Enums;

namespace HomeServer.Application.Dtos.Vehicles
{
    public class VehicleListItemDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = default!;
        public string Model { get; set; } = default!;
        public int Production { get; set; }
        public string Type { get; set; } = default!;
    }
}
