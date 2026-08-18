using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.ParameterChecks;

namespace HomeServer.Application.Interfaces.ParameterChecks
{
    public interface IParameterChecksService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateParameterCheckDto createDto);
        public Task<Result<MessageResponseDto>> CreateForAquariumAsync(Guid aquariumId, CreateParameterCheckForAquariumDto createDto);
        public Task<Result<PagedResult<ParameterCheckResponseDto>>> GetAllAsync(PaginationRequestDto paginationDto, ParameterCheckFilterDto filtrationDto);
        public Task<Result<ParameterCheckEditResponseDto>> GetForEditAsync(Guid parameterCheckId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid parameterCheckId, UpdateParameterCheckDto updateDto);
        public Task<Result<MessageResponseDto>> UpdateForAquariumAsync(Guid parameterCheckId, UpdateParameterCheckForAquariumDto updateDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid parameterCheckId);
    }
}
