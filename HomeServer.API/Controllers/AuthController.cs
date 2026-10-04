using HomeServer.API.Common.Constraints;
using HomeServer.API.Common.Extensions;
using HomeServer.API.Configuration.RateLimiting;
using HomeServer.Application.Configuration.Jwt;
using HomeServer.Application.Dtos.Auth;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Auth;
using HomeServer.Application.Interfaces.UserSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace HomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly JwtOptions _jwtOptions;
        private readonly IWebHostEnvironment _env;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService, IOptions<JwtOptions> jwtOptions, IWebHostEnvironment env)
        {
            _authService = authService;
            _currentUserService = currentUserService;

            _jwtOptions = jwtOptions.Value;
            _env = env;
        }

        [EnableRateLimiting(RateLimitingConstants.Login)]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto loginUserDto)
        {
            var response = await _authService.LoginAsync(loginUserDto);

            // Validate if response success
            if (response.IsFailure)
            {
                return response.ToActionResult(this);
            }

            var loginData = response.Value;

            Response.Cookies.Append(CookieConstants.RefreshTokenCookieKey, loginData.RefreshToken, CreateRefreshTokenCookieOptions());

            return Ok(new LoginResponseDto
            {
                AccessToken = loginData.AccessToken,
                Role = loginData.Role
            });
        }

        [EnableRateLimiting(RateLimitingConstants.Refresh)]
        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshSessionResultDto>> Refresh()
        {
            if (!Request.Cookies.TryGetValue(CookieConstants.RefreshTokenCookieKey, out string? refreshToken))
            {
                return Unauthorized("Brak tokenu odświeżającego.");
            }

            var response = await _authService.RefreshAsync(refreshToken);

            if (response.IsFailure)
            {
                Response.Cookies.Delete(CookieConstants.RefreshTokenCookieKey, CreateRefreshTokenCookieOptions(forDelete: true));
                return response.ToActionResult(this);
            }

            var refreshData = response.Value;

            Response.Cookies.Append(CookieConstants.RefreshTokenCookieKey, refreshData.RefreshToken, CreateRefreshTokenCookieOptions());

            return Ok(new RefreshSessionResultDto
            {
                AccessToken = refreshData.AccessToken
            });
        }

        [EnableRateLimiting(RateLimitingConstants.Refresh)]
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<MessageResponseDto>> Logout()
        {
            if (!Request.Cookies.TryGetValue(CookieConstants.RefreshTokenCookieKey, out string? refreshToken))
            {
                return Unauthorized("Brak tokenu odświeżającego.");
            }

            var response = await _authService.LogoutAsync(refreshToken, _currentUserService.UserId);

            // Invalidates user's refresh token - invalidates session!
            if (response.IsSuccess)
            {
                Response.Cookies.Delete(CookieConstants.RefreshTokenCookieKey, CreateRefreshTokenCookieOptions(forDelete: true));
            }

            return response.ToActionResult(this);
        }


        // Helpers
        private CookieOptions CreateRefreshTokenCookieOptions(bool forDelete = false)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = _env.IsProduction(),
                SameSite = SameSiteMode.Lax,
                Expires = forDelete ? null : DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.Refresh.DurationInMinutes),
                MaxAge = forDelete ? null : TimeSpan.FromMinutes(_jwtOptions.Refresh.DurationInMinutes),
                Path = "/api/auth"
            };
        }
    }
}
