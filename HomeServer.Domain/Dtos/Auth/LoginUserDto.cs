namespace HomeServer.Domain.Dtos.Auth
{
    public class LoginUserDto
    {
        public string Login { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
