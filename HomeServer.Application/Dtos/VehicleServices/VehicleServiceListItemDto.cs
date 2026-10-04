namespace HomeServer.Application.Dtos.VehicleServices
{
    public class VehicleServiceListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public DateOnly Date { get; set; }
        public decimal? Cost { get; set; }
        public int? Mileage { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; } = default!;

        public int ItemsCount { get; set; }
    }
}
