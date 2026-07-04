using HomeServer.Application.Common.Results;
using HomeServer.Domain.Dtos.Users;

namespace HomeServer.Application.Interfaces.Users
{
    public interface IUsersService
    {
        public Task<Result<UserMyProfileDto>> GetUserMyProfile(Guid userId);
    }
}
