using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Users;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Users
{
    public class UsersService : BaseService, IUsersService
    {
        private readonly IMemoryCache _cache;
        private const string USER_CACHE_KEY = "users";

        public UsersService(MyDbContext context, ILogger<UsersService> logger, IMemoryCache cache) : base(context, logger)
        {
            _cache = cache;
        }

        public async Task<Result<UserMyProfileDto>> GetUserMyProfile(Guid userId)
        {
            string cacheKey = $"{USER_CACHE_KEY}:{userId}";

            if (_cache.TryGetValue(cacheKey, out var cachedUserObj) &&
                    cachedUserObj != null &&
                    cachedUserObj is UserMyProfileDto cachedUser
                )
            {
                return Result<UserMyProfileDto>.Success(cachedUser);
            }

            try
            {

                var user = await _context.Users
                    .Where(item => item.Id == userId &&
                        item.DeletedAt == null
                    )
                    .Select(item => new UserMyProfileDto()
                    {
                        Email = item.Email,
                        Name = item.Name,
                        Role = item.Role.ToString(),
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return Result.Failure("Brak użytkownika o podanym identyfikatorze!", ProblemType.NotFound);
                }

                _cache.Set(cacheKey, user, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                });

                return Result<UserMyProfileDto>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get my-profile for user with Id: {UserId}", userId);
                return Result.Failure("Nie można pobrać profilu użytkownika!", ProblemType.InternalServerError);
            }
        }
    }
}
