namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangeForAquariumEditResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateOnly ChangeDate { get; set; }
    }
}
