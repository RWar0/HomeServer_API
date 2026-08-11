namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangeForEditResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly ChangeDate { get; set; }
        public Guid AquariumId { get; set; }
    }
}
