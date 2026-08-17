using HomeServer.Application.Dtos.Pagination;

namespace HomeServer.Application.Common.Results
{
    public sealed record PagedResult<T>
    {
        public List<T> Data { get; set; } = default!;
        public PaginationResponseDto Pagination { get; set; } = default!;
    }
}
