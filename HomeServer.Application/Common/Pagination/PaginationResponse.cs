using HomeServer.Application.Common.Enums;

namespace HomeServer.Application.Common.Pagination
{
    public record PaginationResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (decimal)PageSize) : 0;

        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
