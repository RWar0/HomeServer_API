using HomeServer.Domain.Base;

namespace HomeServer.Domain.Models
{
    public class UserRefreshToken : BaseModel
    {
        public User User { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; } = null;
    }
}
