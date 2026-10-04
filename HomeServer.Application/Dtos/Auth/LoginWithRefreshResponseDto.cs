namespace HomeServer.Application.Dtos.Auth
{
    public class LoginWithRefreshResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
