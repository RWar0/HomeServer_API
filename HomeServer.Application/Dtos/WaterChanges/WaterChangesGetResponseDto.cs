namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangesGetResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly ChangeDate { get; set; }
        public Guid AquariumId { get; set; }
        public string AquariumName { get; set; } = default!;
    }
}
