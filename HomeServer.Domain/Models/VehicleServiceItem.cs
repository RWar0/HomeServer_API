using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class VehicleServiceItem : BaseModel
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public decimal? Cost { get; set; }

        public Guid VehicleServiceId { get; set; }
        public VehicleService VehicleService { get; set; } = default!;
    }
}
