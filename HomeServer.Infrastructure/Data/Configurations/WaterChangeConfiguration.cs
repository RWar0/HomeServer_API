using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class WaterChangeConfiguration : IEntityTypeConfiguration<WaterChange>
    {
        public void Configure(EntityTypeBuilder<WaterChange> builder)
        {
            builder.HasIndex(x => x.AquariumId);
            builder.HasIndex(x => x.ChangeDate);
        }
    }
}
