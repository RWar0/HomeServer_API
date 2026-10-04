using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class CustomEventConfiguration : IEntityTypeConfiguration<CustomEvent>
    {
        public void Configure(EntityTypeBuilder<CustomEvent> builder)
        {
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => new { x.Date, x.Category });
        }
    }
}
