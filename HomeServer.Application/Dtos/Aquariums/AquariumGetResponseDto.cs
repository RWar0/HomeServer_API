namespace HomeServer.Application.Dtos.Aquariums
{
    public class AquariumGetResponseDto
    {
        public string Name { get; set; } = default!;
        public int Volume { get; set; }
        public DateOnly CreationDate { get; set; }
        public Guid? LastPhotoId { get; set; }
    }
}
