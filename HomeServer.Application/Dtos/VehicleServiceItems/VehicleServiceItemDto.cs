namespace HomeServer.Application.Dtos.VehicleServiceItems
{
    public class VehicleServiceItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public decimal? Cost { get; set; }
    }
}
