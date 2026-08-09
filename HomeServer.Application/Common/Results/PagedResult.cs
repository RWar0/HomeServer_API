using HomeServer.Application.Common.Pagination;

namespace HomeServer.Application.Common.Results
{
    public sealed record PagedResult<T>
    {

        public List<T> Data { get; set; } = default!;
        public PaginationResponse Pagination { get; set; } = default!;
    }
}
