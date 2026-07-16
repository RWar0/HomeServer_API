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

        public async Task<Result<LoginResponseWithRefreshDto>> Login(LoginUserDto loginUserDto)
        {

            // Get and Validate user
            var user = await _context.Users
                    .Where(item => item.Username.Equals(loginUserDto.Login) && item.DeletedAt == null)
                    .FirstOrDefaultAsync();

            if (user is null)
            {
                return Result.Failure("Niepoprawny login lub/i hasło!", ProblemType.Unauthorized);
            }


            if (!_passwordHasher.IsPasswordValid(loginUserDto.Password, user.Password))
            {
                return Result.Failure("Niepoprawny login lub/i hasło!", ProblemType.Unauthorized);
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
            return Result<LoginResponseWithRefreshDto>.Success(new LoginResponseWithRefreshDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Role = user.Role.ToString(),
            });
        }

        public async Task<Result<RefreshSessionResponseDto>> Refresh(string oldRefreshToken)
        {
            // Get user from refresh token
            Guid userId;
            try
            {
                userId = _jwtService.GetUserIdFromRefreshToken(oldRefreshToken);
            }
            catch (SecurityTokenException)
            {
                return Result.Failure("Sesja wygasła!", ProblemType.Unauthorized);
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Failure("Niepoprawna sesja!", ProblemType.Unauthorized);
            }

            // Get hash of current refresh token
            string oldHashedRefreshToken = HashRefreshToken(oldRefreshToken);

            // Check for session
            UserRefreshToken? currentSession = await _context.UserRefreshTokens
                    .Include(item => item.User)
                    .Where(item =>
                        item.DeletedAt == null &&
                        item.RevokedAt == null &&
                        item.RefreshToken == oldHashedRefreshToken &&
                        item.User.Id == userId &&
                        item.ExpiresAt > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

            if (currentSession is null)
            {
                return Result.Failure("Niepoprawna sesja!", ProblemType.Unauthorized);
            }

            if (currentSession.PreviousRefreshToken != null && currentSession.PreviousRefreshToken.Equals(oldHashedRefreshToken))
            {
                // TODO: Adds to logs possible vulnerability

                // Revoke all active user sessions
                await RevokeAllUserSessions(userId);

                return Result.Failure("Twoje konto zostało prawdopodobnie przez kogoś skradzione! Sesja tego urządzenia została już zaktualizowana! Wylogowano z wszystkich urządzeń, w celach bezpieczeństwa.", ProblemType.Unauthorized);
            }

            // Rotate refresh token
            try
            {
                // Generate access token
                string accessToken = _jwtService.GenerateAccessToken(currentSession.User);

                // Generate new refresh token
                var (refreshToken, refreshTokenExpiration) = _jwtService.GenerateRefreshToken(currentSession.User);

                string newHashedRefreshToken = HashRefreshToken(refreshToken);

                // Save new refresh token to db
                currentSession.PreviousRefreshToken = oldHashedRefreshToken;
                currentSession.RefreshToken = newHashedRefreshToken;
                currentSession.ExpiresAt = refreshTokenExpiration;
                currentSession.RefreshDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Result<RefreshSessionResponseDto>.Success(
                    new RefreshSessionResponseDto()
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    }
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result.Failure("Ktoś odświeżył sesję w tym samym momencie!", ProblemType.Unauthorized);
            }
            catch
            {
                return Result.Failure("Nie można odświeżyć sesji!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<string>> Logout(string refreshToken, Guid userId)
        {
            var hashedRefreshToken = HashRefreshToken(refreshToken);

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
                return Result.Failure("Niepoprawna sesja!");
            }

            session.RevokedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Result<string>.Success("Wylogowano poprawnie!");
        }

        private async Task RevokeAllUserSessions(Guid userId)
        {
            UserRefreshToken[] allUserSessions = await _context.UserRefreshTokens
                .Where(item =>
                    item.DeletedAt == null &&
                    item.RevokedAt == null &&
                    item.User.Id == userId &&
                    item.ExpiresAt > DateTime.UtcNow)
                .ToArrayAsync();

            if (allUserSessions.Length == 0)
            {
                return;
            }

            foreach (var session in allUserSessions)
            {
                session.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
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
