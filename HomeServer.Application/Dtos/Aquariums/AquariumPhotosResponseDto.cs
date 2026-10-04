namespace HomeServer.Application.Dtos.Aquariums
{
    public class AquariumPhotosResponseDto
    {
        public Guid Id { get; set; }
        public string OriginalName { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
