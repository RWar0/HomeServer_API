using HomeServer.Application.Configuration.Jwt;
using HomeServer.Application.Interfaces.Auth;
using HomeServer.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeServer.Application.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        private JwtOptions _jwtOptions;

        public JwtService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Access.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimTypes.Name, user.Username),
            ];

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.Access.DurationInMinutes),
                signingCredentials: credentials);

            return _tokenHandler.WriteToken(token);
        }

        public (string refreshToken, DateTime refreshTokenExpiration) GenerateRefreshToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Refresh.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(ClaimTypes.Name, user.Username),
            ];

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.Refresh.DurationInMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return (_tokenHandler.WriteToken(token), expiresAt);
        }

        public Guid GetUserIdFromRefreshToken(string refreshToken)
        {
            var principal = GetPrincipalFromToken(refreshToken, _jwtOptions.Refresh.Key);

            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User from session is invalid!"); ;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user id format in token!");
            }

            return userId;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(signingKey);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            var claims = tokenHandler.ValidateToken(token, parameters, out _);
            return claims;
        }
    }
}
