using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Aquariums;

namespace HomeServer.Application.Interfaces.Aquariums
{
    public interface IAquariumsService
    {
        public Task<Result<string>> Create(AquariumCreateDto createDto);
        public Task<Result<AquariumGetResponseDto[]>> GetAll(BasePaginationDto paginationDto);
    }
}
