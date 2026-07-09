using HomeServer.API.Common.Extensions;
using HomeServer.API.Configuration.RateLimiting;
using HomeServer.Application.Dtos.Auth;
using HomeServer.Application.Interfaces.Auth;
using HomeServer.Application.Interfaces.UserSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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

        [EnableRateLimiting(RateLimitingConstants.Login)]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDto loginUserDto)
        {
            var response = await _authService.Login(loginUserDto);

            return response.ToActionResult(this);
        }

        [EnableRateLimiting(RateLimitingConstants.Refresh)]
        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshSessionResponseDto>> Refresh(RefreshSessionDto refreshSessionDto)
        {
            var response = await _authService.Refresh(refreshSessionDto);

            return response.ToActionResult(this);
        }

        [EnableRateLimiting(RateLimitingConstants.Refresh)]
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<string>> Logout(LogoutDto logoutDto)
        {
            var response = await _authService.Logout(logoutDto, _currentUserService.UserId);

            return response.ToActionResult(this);
        }
    }
}
