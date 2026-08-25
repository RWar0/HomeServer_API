using HomeServer.Domain.Base;
using HomeServer.Domain.Enums;

namespace HomeServer.Domain.Models
{
    public class Vehicle : BaseModel
    {
        public string Brand { get; set; } = default!;
        public string Model { get; set; } = default!;
        public int Production { get; set; }
        public VehiclesEnum Type { get; set; }

        public ICollection<VehicleFueling> Fuelings { get; set; } = [];
    }
}
