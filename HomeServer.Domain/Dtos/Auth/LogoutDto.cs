namespace HomeServer.Domain.Dtos.Auth
{
    public class LogoutDto
    {
        public string RefreshToken { get; set; } = default!;
    }
}
