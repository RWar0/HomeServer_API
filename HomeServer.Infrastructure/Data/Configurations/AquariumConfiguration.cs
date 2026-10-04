using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class AquariumConfiguration : IEntityTypeConfiguration<Aquarium>
    {
        public void Configure(EntityTypeBuilder<Aquarium> builder)
        {
            builder
                .HasMany(x => x.WaterChanges)
                .WithOne(x => x.Aquarium)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.Photos)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
