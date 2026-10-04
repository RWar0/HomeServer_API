using HomeServer.Domain.Enums;

namespace HomeServer.Application.Dtos.Users
{
    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public RolesEnum Role { get; set; }
    }
}
