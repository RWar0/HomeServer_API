namespace HomeServer.Application.Dtos.ParameterChecks
{
    public class ParameterCheckFilterDto
    {
        public Guid? AquariumId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
