using HomeServer.Application.Interfaces.Auth;
using HomeServer.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeServer.Application.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        private readonly string _accessTokenKey;
        private readonly int _accessTokenDuration;

        private readonly string _refreshTokenKey;
        private readonly int _refreshTokenDuration;

        public JwtService(IConfiguration config)
        {
            _jwtIssuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Cannot get jwt issuer!");
            _jwtAudience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Cannot get jwt audience!");

            _accessTokenKey = config["Jwt:Access:Key"] ?? throw new InvalidOperationException("Cannot get access token key!");
            _accessTokenDuration = GetRequiredInt(config, "Jwt:Access:DurationInMinutes");

            _refreshTokenKey = config["Jwt:Refresh:Key"] ?? throw new InvalidOperationException("Cannot get refresh token key!");
            _refreshTokenDuration = GetRequiredInt(config, "Jwt:Refresh:DurationInMinutes");
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
            ];

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenDuration),
                signingCredentials: credentials);

            return _tokenHandler.WriteToken(token);
        }

        public (string refreshToken, DateTime refreshTokenExpiration) GenerateRefreshToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshTokenKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
            ];

            var expiresAt = DateTime.UtcNow.AddMinutes(_refreshTokenDuration);

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return (_tokenHandler.WriteToken(token), expiresAt);
        }

        public Guid GetUserIdFromRefreshToken(string refreshToken)
        {
            var principal = GetPrincipalFromToken(refreshToken, _refreshTokenKey);

            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User from session is invalid!"); ;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user id format in token!");
            }

            return userId;
        }

        private static int GetRequiredInt(IConfiguration config, string path)
        {
            if (!int.TryParse(config[path], out var value))
            {
                throw new InvalidOperationException($"Invalid value in configuration file: {path}");
            }

            return value;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(signingKey);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,

                ValidateAudience = true,
                ValidAudience = _jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateLifetime = true,
            };

            var claims = tokenHandler.ValidateToken(token, parameters, out _);
            return claims;
        }
    }
}
