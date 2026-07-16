namespace HomeServer.Application.Dtos.Auth
{
    public class LoginResponseWithRefreshDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
