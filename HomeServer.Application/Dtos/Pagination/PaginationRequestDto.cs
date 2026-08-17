using HomeServer.Application.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace HomeServer.Application.Dtos.Pagination
{
    public class PaginationRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Strona musi być dodatnia!")]
        public int Page { get; set; } = 1;
        [Range(10, 100, ErrorMessage = "Rozmiar strony musi zawierać się w zakresie 10-100!")]
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    }
}
