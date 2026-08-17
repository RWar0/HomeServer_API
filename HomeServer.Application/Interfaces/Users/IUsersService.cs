using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Users;

namespace HomeServer.Application.Interfaces.Users
{
    public interface IUsersService
    {
        public Task<Result<MyProfileResponseDto>> GetMyProfileAsync(Guid userId);
    }
}
