namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangeDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly ChangeDate { get; set; }
        public string AquariumName { get; set; } = default!;
    }
}
