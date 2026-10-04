namespace HomeServer.Application.Dtos.Dashboar
{
    public class DashboardRecentActivityDto
    {
        public Guid Id { get; set; }
        public string? Category { get; set; } = default!;
        public string Title { get; set; } = default!;
        public DateOnly Date { get; set; }
    }
}
