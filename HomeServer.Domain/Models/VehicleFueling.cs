using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class VehicleFueling : BaseModel
    {
        public double Quantity { get; set; }
        public decimal? Cost { get; set; }
        public DateOnly Date { get; set; }

        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = default!;
    }
}