using HomeServer.Domain.Models;

namespace HomeServer.Application.Interfaces.Auth
{
    public interface IJwtService
    {
        public string GenerateAccessToken(User user);

        public Guid GetUserIdFromRefreshToken(string refreshToken);

        public (string refreshToken, DateTime refreshTokenExpiration) GenerateRefreshToken(User user);
    }
}
