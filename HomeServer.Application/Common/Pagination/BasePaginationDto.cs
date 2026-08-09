namespace HomeServer.Application.Common.Pagination
{
    public class BasePaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
