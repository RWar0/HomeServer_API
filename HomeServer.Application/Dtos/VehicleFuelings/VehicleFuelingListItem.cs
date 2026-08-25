namespace HomeServer.Application.Dtos.VehicleFuelings
{
    public class VehicleFuelingListItem
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public decimal? Cost { get; set; }
        public DateOnly Date { get; set; }
        public Guid VehicleId { get; set; }
        public string VehicleName { get; set; } = default!;
    }
}