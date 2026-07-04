namespace HomeServer.Domain.Dtos.Auth
{
    public class RefreshSessionResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
