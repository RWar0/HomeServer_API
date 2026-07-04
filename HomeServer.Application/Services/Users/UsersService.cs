using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Domain.Dtos.Users;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HomeServer.Application.Services.Users
{
    public class UsersService : BaseService, IUsersService
    {
        private readonly IMemoryCache _cache;

        public UsersService(MyDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public async Task<Result<UserMyProfileDto>> GetUserMyProfile(Guid userId)
        {
            string cacheKey = $"user:{userId}";

            if (_cache.TryGetValue(cacheKey, out var cachedUserObj) &&
                    cachedUserObj != null &&
                    cachedUserObj is UserMyProfileDto cachedUser
                )
            {
                return Result<UserMyProfileDto>.Success(cachedUser);
            }

            var user = await _context.Users
                .Where(item => item.Id == userId &&
                    item.DeletedAt == null
                )
                .Select(item => new UserMyProfileDto()
                {
                    Email = item.Email,
                    Name = item.Name
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result<UserMyProfileDto>.Failure("Brak użytkownika o podanym identyfikatorze!");
            }

            _cache.Set(cacheKey, user, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            });

            return Result<UserMyProfileDto>.Success(user);
        }
    }
}
