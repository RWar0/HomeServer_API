namespace HomeServer.Application.Dtos.WaterChanges
{
    public class WaterChangeListFiltrationDto
    {
        public Guid? AquariumId { get; set; }
        public DateOnly? fromDate { get; set; }
        public DateOnly? toDate { get; set; }
    }
}
