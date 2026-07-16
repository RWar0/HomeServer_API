using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Auth;

namespace HomeServer.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<Result<LoginResponseWithRefreshDto>> Login(LoginUserDto loginUserDto);
        public Task<Result<RefreshSessionResponseDto>> Refresh(string oldRefreshToken);
        public Task<Result<string>> Logout(string refreshToken, Guid userId);
    }
}
