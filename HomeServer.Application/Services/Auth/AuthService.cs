using System.Text;
using HomeServer.Application.Base;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Auth;
using HomeServer.Application.Interfaces.Auth;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HomeServer.Application.Services.Auth
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(MyDbContext context, IPasswordHasher passwordHasher, IJwtService jwtService) : base(context)
        {
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<Result<LoginResponseDto>> Login(LoginUserDto loginUserDto)
        {

            // Get and Validate user
            var user = await _context.Users
                    .Where(item => item.Username.Equals(loginUserDto.Login) && item.DeletedAt == null)
                    .FirstOrDefaultAsync();

            if (user is null)
            {
                return Result<LoginResponseDto>.Failure("Niepoprawny login lub/i hasło!");
            }


            if (!_passwordHasher.IsPasswordValid(loginUserDto.Password, user.Password))
            {
                return Result<LoginResponseDto>.Failure("Niepoprawny login lub/i hasło!");
            }

            // Generate access and refresh tokens
            string accessToken = _jwtService.GenerateAccessToken(user);
            var (refreshToken, refreshTokenExpiration) = _jwtService.GenerateRefreshToken(user);

            // Save refresh token to db
            string hashedToken = HashRefreshToken(refreshToken);

            _context.UserRefreshTokens.Add(new UserRefreshToken()
            {
                RefreshToken = hashedToken,
                User = user,
                ExpiresAt = refreshTokenExpiration
            });

            await _context.SaveChangesAsync();

            // Return results
            return Result<LoginResponseDto>.Success(new LoginResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = user.Role.ToString(),
            });
        }

        public async Task<Result<RefreshSessionResponseDto>> Refresh(RefreshSessionDto refreshSessionDto)
        {
            // Get user from refresh token
            Guid userId;
            try
            {
                userId = _jwtService.GetUserIdFromRefreshToken(refreshSessionDto.RefreshToken);
            } catch (SecurityTokenException)
            {
                return Result<RefreshSessionResponseDto>.Failure("Niepoprawna sesja!");
            } catch (UnauthorizedAccessException)
            {
                return Result<RefreshSessionResponseDto>.Failure("Niepoprawna sesja!");
            }

            // Get hash of current refresh token
            string oldHashedRefreshToken = HashRefreshToken(refreshSessionDto.RefreshToken);

            // Check for session
            UserRefreshToken? session = await _context.UserRefreshTokens
                    .Include(item => item.User)
                    .Where(item =>
                        item.DeletedAt == null &&
                        item.RevokedAt == null &&
                        item.RefreshToken == oldHashedRefreshToken &&
                        item.User.Id == userId &&
                        item.ExpiresAt > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

            if (session is null)
            {
                return Result<RefreshSessionResponseDto>.Failure("Niepoprawna sesja!");
            }

            // Revoke session - invalidates old and insert new
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Generate access token
                string accessToken = _jwtService.GenerateAccessToken(session.User);

                // Generate new refresh token
                var (refreshToken, refreshTokenExpiration) = _jwtService.GenerateRefreshToken(session.User);

                string newHashedRefreshToken = HashRefreshToken(refreshToken);

                // Invalidates old session
                session.RevokedAt = DateTime.UtcNow;

                // Save new refresh token to db
                _context.UserRefreshTokens.Add(new UserRefreshToken()
                {
                    RefreshToken = newHashedRefreshToken,
                    User = session.User,
                    ExpiresAt = refreshTokenExpiration
                });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                ;
                return Result<RefreshSessionResponseDto>.Success(
                    new RefreshSessionResponseDto()
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                );
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new DbUpdateException("Nie można odświeżyć sesji!");
            }
        }

        public async Task<Result<string>> Logout(LogoutDto logoutDto, Guid userId)
        {
            var hashedRefreshToken = HashRefreshToken(logoutDto.RefreshToken);

            UserRefreshToken? session = await _context.UserRefreshTokens
                .Include(item => item.User)
                .Where(item =>
                    item.DeletedAt == null &&
                    item.RevokedAt == null &&
                    item.RefreshToken == hashedRefreshToken &&
                    item.User.Id == userId &&
                    item.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync();

            if (session is null)
            {
                return Result<string>.Failure("Niepoprawna sesja!");
            }

            session.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Result<string>.Success("Wylogowano poprawnie!");
        }

        // Helper methods
        private static string HashRefreshToken(string refreshToken)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            var hashedToken = Convert.ToBase64String(bytes);

            return hashedToken;
        }
    }
}
