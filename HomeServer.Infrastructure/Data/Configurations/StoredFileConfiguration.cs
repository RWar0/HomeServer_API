using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
    {
        public void Configure(EntityTypeBuilder<StoredFile> builder)
        {
            builder.HasIndex(x => x.RelativePath)
                .IsUnique();
            builder.HasIndex(x => x.OriginalName);
        }
    }
}
