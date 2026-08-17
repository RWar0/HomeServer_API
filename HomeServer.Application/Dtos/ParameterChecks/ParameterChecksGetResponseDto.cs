namespace HomeServer.Application.Dtos.ParameterChecks
{
    public class ParameterChecksGetResponseDto
    {
        public Guid Id { get; set; }
        public double? Ph { get; set; }
        public double? Kh { get; set; }
        public double? Gh { get; set; }
        public double? No3 { get; set; }
        public double? No2 { get; set; }
        public double? Temperature { get; set; }
        public DateOnly MeasuredAt { get; set; }

        public Guid AquariumId { get; set; }
        public string AquariumName { get; set; } = default!;
    }
}
