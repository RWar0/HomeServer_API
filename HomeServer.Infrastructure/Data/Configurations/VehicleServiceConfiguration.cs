using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class VehicleServiceConfiguration : IEntityTypeConfiguration<VehicleService>
    {
        public void Configure(EntityTypeBuilder<VehicleService> builder)
        {
            builder.HasIndex(x => x.VehicleId);
            builder.HasIndex(x => x.Date);
            builder.HasIndex(x => new { x.VehicleId, x.Date });
        }
    }
}
