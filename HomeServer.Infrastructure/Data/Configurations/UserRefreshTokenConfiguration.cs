using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasIndex(x => x.RefreshToken);
            builder.HasIndex(x => x.ExpiresAt);
            builder.HasIndex(x => x.RevokedAt);
        }
    }
}
