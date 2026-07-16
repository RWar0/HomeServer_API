namespace HomeServer.Application.Configuration.Jwt
{
    public class JwtOptions
    {
        public AccessTokenOptions Access { get; set; } = new();
        public RefreshTokenOptions Refresh { get; set; } = new();
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }

    public class AccessTokenOptions
    {
        public string Key { get; set; } = default!;
        public int DurationInMinutes { get; set; }
    }

    public class RefreshTokenOptions
    {
        public string Key { get; set; } = default!;
        public int DurationInMinutes { get; set; }
    }
}
