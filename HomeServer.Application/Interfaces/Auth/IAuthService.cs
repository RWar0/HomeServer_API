using HomeServer.Application.Common.Results;
using HomeServer.Domain.Dtos.Auth;

namespace HomeServer.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        public Task<Result<LoginResponseDto>> Login(LoginUserDto loginUserDto);

        public Task<Result<RefreshSessionResponseDto>> Refresh(RefreshSessionDto refreshSessionDto);
        public Task<Result<string>> Logout(LogoutDto logoutDto, Guid userId);
    }
}
