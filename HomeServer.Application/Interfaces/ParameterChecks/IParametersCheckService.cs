using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.ParameterChecks;

namespace HomeServer.Application.Interfaces.ParameterChecks
{
    public interface IParametersCheckService
    {
        public Task<Result<MessageResponseDto>> Create(ParameterCheckCreateEditDto createDto);
        public Task<Result<PagedResult<ParameterChecksGetResponseDto>>> GetAll(PaginationDto paginationDto, ParameterChecksListFiltrationDto filtrationDto);
        public Task<Result<ParameterCheckEditResponseDto>> GetForEdit(Guid parameterCheckId);
        public Task<Result<MessageResponseDto>> Update(Guid parameterCheckId, ParameterCheckCreateEditDto editDto);
        public Task<Result<MessageResponseDto>> Delete(Guid parameterCheckId);
    }
}
