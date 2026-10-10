namespace HomeServer.Application.Dtos.ParameterChecks
{
    public class ParameterCheckDto
    {
        public Guid Id { get; set; }
        public double? Ph { get; set; }
        public double? Kh { get; set; }
        public double? Gh { get; set; }
        public double? No3 { get; set; }
        public double? No2 { get; set; }
        public double? Nh3 { get; set; }
        public double? Po4 { get; set; }
        public double? Fe { get; set; }
        public double? Temperature { get; set; }

        public DateOnly MeasuredAt { get; set; }
        public TimeOnly? MeasuredTime { get; set; }

        public string AquariumName { get; set; } = default!;
    }
}
