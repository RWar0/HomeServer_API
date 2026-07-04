namespace HomeServer.Domain.Dtos.Auth
{
    public class RefreshSessionDto
    {
        public string RefreshToken { get; set; } = default!;
    }
}
