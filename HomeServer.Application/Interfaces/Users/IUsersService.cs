using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Users;

namespace HomeServer.Application.Interfaces.Users
{
    public interface IUsersService
    {
        public Task<Result<MessageResponseDto>> CreateAsync(CreateUserDto createDto);
        public Task<Result<MyProfileResponseDto>> GetMyProfileAsync(Guid userId);
        public Task<Result<PagedResult<UserListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto);
        public Task<Result<UserEditDto>> GetForEditAsync(Guid userId);
        public Task<Result<MessageResponseDto>> UpdateAsync(Guid userId, UpdateUserDto updateDto);
        public Task<Result<MessageResponseDto>> UpdatePasswordAsync(Guid userId, UpdateUserPasswordDto updatePasswordDto);
        public Task<Result<MessageResponseDto>> DeleteAsync(Guid userToDeleteId, Guid myUserId);
    }
}
