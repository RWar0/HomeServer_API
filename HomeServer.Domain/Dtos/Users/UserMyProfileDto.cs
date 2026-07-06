namespace HomeServer.Domain.Dtos.Users
{
    public class UserMyProfileDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
