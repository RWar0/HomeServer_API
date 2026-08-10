using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class WaterChange : BaseModel
    {
        public decimal Amount { get; set; }
        public DateOnly ChangeDate { get; set; }

        public Aquarium Aquarium { get; set; } = default!;
    }
}
