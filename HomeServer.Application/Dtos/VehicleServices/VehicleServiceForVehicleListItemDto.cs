namespace HomeServer.Application.Dtos.VehicleServices
{
    public class VehicleServiceForVehicleListItemDto
    {
        public string Title { get; set; } = default!;
        public DateOnly Date { get; set; }
        public decimal? Cost { get; set; }

        public int ItemsCount { get; set; }
    }
}
