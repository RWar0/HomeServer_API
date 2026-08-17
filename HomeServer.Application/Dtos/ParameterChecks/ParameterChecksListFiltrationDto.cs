namespace HomeServer.Application.Dtos.ParameterChecks
{
    public class ParameterChecksListFiltrationDto
    {
        public Guid? AquariumId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
