namespace HomeServer.Application.Dtos.Dashboar
{
    public class DashboardSummaryDto
    {
        public int TodayEventsCount { get; set; }
        public int OverdueEventsCount { get; set; }
        public int? RemainingDaysToNextEvent { get; set; }
        public string? RemainingEventTitle { get; set; }
        public int TodayCompletedEventsCount { get; set; }
    }
}
