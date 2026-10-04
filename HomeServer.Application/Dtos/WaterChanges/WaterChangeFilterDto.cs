namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangeFilterDto
    {
        public Guid? AquariumId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
