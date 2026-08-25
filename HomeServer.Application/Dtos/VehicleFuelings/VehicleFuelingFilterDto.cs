namespace HomeServer.Application.Dtos.VehicleFuelings
{
    public class VehicleFuelingFilterDto
    {
        public Guid? VehicleId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
