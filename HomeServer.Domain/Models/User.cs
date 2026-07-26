using HomeServer.Domain.Base;
using HomeServer.Domain.Enums;

namespace HomeServer.Domain.Models
{
    public class User : BaseModel
    {
        public string Name { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Email { get; set; } = default!;

        public RolesEnum Role { get; set; } = RolesEnum.User;

        public ICollection<UserRefreshToken> RefreshTokens { get; set; } = [];
    }
}
