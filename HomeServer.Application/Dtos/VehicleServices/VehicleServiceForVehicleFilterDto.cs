namespace HomeServer.Application.Dtos.VehicleServices
{
    public class VehicleServiceForVehicleFilterDto
    {
        public Guid? VehicleId { get; set; }
        public string? Title { get; set; }
        public decimal? FromCost { get; set; }
        public decimal? ToCost { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
