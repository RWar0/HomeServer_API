namespace HomeServer.Application.Dtos.VehicleFuelings
{
    public class VehicleFuelingsForVehicleListItemDto
    {
        public Guid Id { get; set; }
        public double Quantity { get; set; }
        public decimal? Cost { get; set; }
        public DateOnly Date { get; set; }
    }
}
