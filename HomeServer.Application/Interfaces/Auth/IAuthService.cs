using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Auth;
using HomeServer.Application.Dtos.Common;

namespace HomeServer.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<Result<LoginResponseWithRefreshDto>> Login(LoginUserDto loginUserDto);
        public Task<Result<RefreshSessionResponseDto>> Refresh(string oldRefreshToken);
        public Task<Result<MessageResponseDto>> Logout(string refreshToken, Guid userId);
    }
}
