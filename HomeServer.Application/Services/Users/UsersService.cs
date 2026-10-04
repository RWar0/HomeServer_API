using HomeServer.Application.Base;
using HomeServer.Application.Common.Enums;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Dtos.Pagination;
using HomeServer.Application.Dtos.Users;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Services.Users
{
    public class UsersService : BaseService, IUsersService
    {
        private readonly IMemoryCache _cache;
        private readonly IPasswordHasher _passwordHasher;
        private const string USER_CACHE_KEY = "users";

        public UsersService(MyDbContext context, ILogger<UsersService> logger, IMemoryCache cache, IPasswordHasher passwordHasher) : base(context, logger)
        {
            _cache = cache;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<MessageResponseDto>> CreateAsync(CreateUserDto createDto)
        {
            try
            {
                if (!createDto.Password.Equals(createDto.ConfirmPassword))
                {
                    return Result.Failure("Podane hasła się różnią!", ProblemType.BadRequest);
                }

                if (await _context.Users.AnyAsync(u => u.Username == createDto.Username))
                {
                    return Result.Failure("Użytkownik o podanym loginie już istnieje!", ProblemType.BadRequest);
                }

                if (await _context.Users.AnyAsync(u => u.Email == createDto.Email))
                {
                    return Result.Failure("Użytkownik o podanym adresie e-mail już istnieje!", ProblemType.BadRequest);
                }

                var hashedPassword = _passwordHasher.GetHashedPassword(createDto.Password);

                _context.Users.Add(new User
                {
                    Name = createDto.Name,
                    Username = createDto.Username,
                    Email = createDto.Email,
                    Password = hashedPassword,
                    Role = createDto.Role,
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto
                {
                    Message = $"Użytkownik \"{createDto.Name}\" został pomyślnie dodany!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot create user {Name}", createDto.Name);
                return Result.Failure("Nie dodano użytkownika do bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MyProfileResponseDto>> GetMyProfileAsync(Guid userId)
        {
            string cacheKey = $"{USER_CACHE_KEY}:{userId}";

            if (_cache.TryGetValue(cacheKey, out var cachedUserObj) &&
                    cachedUserObj != null &&
                    cachedUserObj is MyProfileResponseDto cachedUser
                )
            {
                return Result<MyProfileResponseDto>.Success(cachedUser);
            }

            try
            {

                var user = await _context.Users
                    .Where(item => item.Id == userId)
                    .Select(item => new MyProfileResponseDto()
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

                return Result<MyProfileResponseDto>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get my-profile for user with Id: {UserId}", userId);
                return Result.Failure("Nie można pobrać profilu użytkownika!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<PagedResult<UserListItemDto>>> GetAllAsync(PaginationRequestDto paginationDto)
        {
            try
            {
                var baseQuery = _context.Users.AsNoTracking();

                var totalCount = await baseQuery.CountAsync();

                baseQuery = ApplySortingForUsers(baseQuery, paginationDto);

                var users = await baseQuery
                    .Skip((paginationDto.Page - 1) * paginationDto.PageSize)
                    .Take(paginationDto.PageSize)
                    .Select(item => new UserListItemDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Username = item.Username,
                        Email = item.Email,
                        Role = item.Role,
                    })
                    .ToListAsync();

                return Result<PagedResult<UserListItemDto>>.Success(new()
                {
                    Data = users,
                    Pagination = new()
                    {
                        Page = paginationDto.Page,
                        PageSize = paginationDto.PageSize,
                        TotalCount = totalCount,
                        SortBy = paginationDto.SortBy,
                        SortDirection = paginationDto.SortDirection,
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get list of all users");
                return Result.Failure("Można pobrać użytkowników z bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<UserEditDto>> GetForEditAsync(Guid userId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(item => item.Id == userId)
                    .Select(item => new UserEditDto
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Username = item.Username,
                        Email = item.Email,
                        Role = item.Role
                    })
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    return Result.Failure("Nie znaleziono użytkownika z podanym identyfikatorem!", ProblemType.NotFound);
                }

                return Result<UserEditDto>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot get user for edit with Id: {UserId}", userId);
                return Result.Failure("Nie można pobrać danych podanego użytkownika!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdateAsync(Guid userId, UpdateUserDto updateDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user is null)
                {
                    return Result.Failure("Nie znaleziono użytkownika z podanym identyfikatorem!", ProblemType.NotFound);
                }

                if (await _context.Users.AnyAsync(u => u.Id != userId && u.Username == updateDto.Username))
                {
                    return Result.Failure("Użytkownik o podanym loginie już istnieje!", ProblemType.BadRequest);
                }

                if (await _context.Users.AnyAsync(u => u.Id != userId && u.Email == updateDto.Email))
                {
                    return Result.Failure("Użytkownik o podanym adresie e-mail już istnieje!", ProblemType.BadRequest);
                }

                user.Name = updateDto.Name;
                user.Username = updateDto.Username;
                user.Email = updateDto.Email;
                user.Role = updateDto.Role;

                await _context.SaveChangesAsync();

                DeleteUserFromCache(userId);

                return Result.Success(new MessageResponseDto
                {
                    Message = "Użytkownik został pomyślnie edytowany!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update user with Id {UserId}", userId);
                return Result.Failure("Nie udało się zaktualizować użytkownika!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> UpdatePasswordAsync(Guid userId, UpdateUserPasswordDto updatePasswordDto)
        {
            try
            {
                if (updatePasswordDto.Password != updatePasswordDto.ConfirmPassword)
                {
                    return Result.Failure("Podane hasła się różnią!", ProblemType.BadRequest);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user is null)
                {
                    return Result.Failure("Nie znaleziono użytkownika z podanym identyfikatorem!", ProblemType.NotFound);
                }

                user.Password = _passwordHasher.GetHashedPassword(updatePasswordDto.Password);

                await _context.SaveChangesAsync();

                DeleteUserFromCache(userId);

                return Result.Success(new MessageResponseDto
                {
                    Message = "Hasło użytkownika zostało pomyślnie zmienione!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot update password of user with Id {UserId}", userId);
                return Result.Failure("Nie udało się zmienić hasła użytkownika!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<MessageResponseDto>> DeleteAsync(Guid userToDeleteId, Guid myUserId)
        {
            try
            {
                if (userToDeleteId == myUserId)
                {
                    return Result.Failure("Nie można usunąć własnego użytkownika!", ProblemType.Conflict);
                }

                var userToDelete = await _context.Users.FirstOrDefaultAsync(item => item.Id == userToDeleteId);

                if (userToDelete is null)
                {
                    return Result.Failure("Nie znaleziono użytkownika z podanym identyfikatorem!", ProblemType.NotFound);
                }

                _context.Remove(userToDelete);

                await _context.SaveChangesAsync();

                DeleteUserFromCache(userToDeleteId);

                return Result.Success(new MessageResponseDto
                {
                    Message = "Użytkownik został poprawnie usunięty!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cannot delete user with Id: {UserId} by user with Id: {MyUserId}", userToDeleteId, myUserId);
                return Result.Failure("Nie można usunąć użytkownika z bazy danych!", ProblemType.InternalServerError);
            }
        }

        // Helpers
        private static IQueryable<User> ApplySortingForUsers(IQueryable<User> query, PaginationRequestDto paginationDto)
        {
            return paginationDto.SortBy?.ToLower() switch
            {
                "name" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Name).ThenBy(x => x.Id)
                    : query.OrderBy(x => x.Name).ThenBy(x => x.Id),

                "username" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Username)
                    : query.OrderBy(x => x.Username),

                "email" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Email)
                    : query.OrderBy(x => x.Email),

                "role" => paginationDto.SortDirection == SortDirection.Desc
                    ? query.OrderByDescending(x => x.Role).ThenBy(x => x.Id)
                    : query.OrderBy(x => x.Role).ThenBy(x => x.Id),

                _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id)
            };
        }

        private void DeleteUserFromCache(Guid userId)
        {
            string cacheKey = $"{USER_CACHE_KEY}:{userId}";

            _cache.Remove(cacheKey);
        }

    }
}
