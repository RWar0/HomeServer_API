using HomeServer.Application.Interfaces.UserSession;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HomeServer.Application.Services.UserSession
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("Invalid session's user Id!");

                if (!Guid.TryParse(userIdString, out var userId))
                {
                    throw new UnauthorizedAccessException("Invalid user id format in token!");
                }

                return userId;
            }
        }

        public string Username =>
            _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.Name)?.Value ?? throw new Exception("Invalid session's username!");

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? throw new Exception("Invalid session's authentication!");
    }
}
