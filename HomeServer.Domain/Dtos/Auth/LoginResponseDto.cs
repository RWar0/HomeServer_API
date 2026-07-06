namespace HomeServer.Domain.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
