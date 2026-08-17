namespace HomeServer.Application.Dtos.Users
{
    public class MyProfileResponseDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
