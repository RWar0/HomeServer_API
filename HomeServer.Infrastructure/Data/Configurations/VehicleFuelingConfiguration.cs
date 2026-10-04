using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class VehicleFuelingConfiguration : IEntityTypeConfiguration<VehicleFueling>
    {
        public void Configure(EntityTypeBuilder<VehicleFueling> builder)
        {
            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.Date);
        }
    }
}
