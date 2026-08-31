using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class VehicleServiceItemConfiguration : IEntityTypeConfiguration<VehicleServiceItem>
    {
        public void Configure(EntityTypeBuilder<VehicleServiceItem> builder)
        {
            builder.HasIndex(x => x.VehicleServiceId);
            builder.HasIndex(x => x.Title);
            builder.HasIndex(x => new { x.VehicleServiceId, x.Title });
        }
    }
}
