using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class VehicleService : BaseModel
    {
        public string Title { get; set; } = default!;
        public DateOnly Date { get; set; }
        public int? Mileage { get; set; }
        public decimal? Cost { get; set; }

        public Guid VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = default!;

        public ICollection<VehicleServiceItem> Items { get; set; } = [];
    }
}
