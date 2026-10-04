using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class Aquarium : BaseModel
    {
        public string Name { get; set; } = default!;
        public int Volume { get; set; }
        public DateOnly CreationDate { get; set; }

        public ICollection<StoredFile> Photos { get; set; } = [];
        public ICollection<WaterChange> WaterChanges { get; set; } = [];
        public ICollection<ParameterCheck> ParameterChecks { get; set; } = [];
    }
}
