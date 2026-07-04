using HomeServer.Application.Interfaces.Auth;
using HomeServer.Application.Interfaces.UserSession;
using HomeServer.Domain.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDto loginUserDto)
        {
            var response = await _authService.Login(loginUserDto);

            if (!response.IsSuccess || response.Value == null)
            {
                return Unauthorized(response.Error);
            }

            return response.Value;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshSessionResponseDto>> Refresh(RefreshSessionDto refreshSessionDto)
        {
            var response = await _authService.Refresh(refreshSessionDto);

            if (!response.IsSuccess || response.Value == null)
            {
                return Unauthorized(response.Error);
            }

            return response.Value;
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<string>> Logout(LogoutDto logoutDto)
        {
            var response = await _authService.Logout(logoutDto, _currentUserService.UserId);

            if (!response.IsSuccess || response.Value == null)
            {
                return Unauthorized(response.Error);
            }

            return response.Value;
        }
    }
}
