namespace HomeServer.Application.Dtos.Common
{
    public class DateFilterDto
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
