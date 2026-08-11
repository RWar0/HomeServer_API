using HomeServer.Application.Common.Enums;

namespace HomeServer.Application.Common.Pagination
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    }
}
