namespace HomeServer.Application.Common.Pagination
{
    public class BasePaginationDto
    {
        public int? CurrentPage { get; set; } = 1;
        public int? ItemsPerPage { get; set; } = 20;
    }
}
