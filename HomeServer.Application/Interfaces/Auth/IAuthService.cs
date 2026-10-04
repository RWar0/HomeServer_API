using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Auth;
using HomeServer.Application.Dtos.Common;

namespace HomeServer.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<Result<LoginWithRefreshResponseDto>> LoginAsync(LoginRequestDto loginUserDto);
        public Task<Result<RefreshSessionResponseDto>> RefreshAsync(string oldRefreshToken);
        public Task<Result<MessageResponseDto>> LogoutAsync(string refreshToken, Guid userId);
    }
}
