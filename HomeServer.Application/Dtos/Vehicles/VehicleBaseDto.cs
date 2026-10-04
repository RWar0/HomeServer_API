namespace HomeServer.Application.Dtos.Vehicles
{
    public class VehicleBaseDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = default!;
        public string Model { get; set; } = default!;
        public int Production { get; set; }
    }
}
