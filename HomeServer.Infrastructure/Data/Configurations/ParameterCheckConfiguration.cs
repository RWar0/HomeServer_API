using HomeServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeServer.Infrastructure.Data.Configurations
{
    public class ParameterCheckConfiguration : IEntityTypeConfiguration<ParameterCheck>
    {
        public void Configure(EntityTypeBuilder<ParameterCheck> builder)
        {
            builder.HasIndex(x => x.MeasuredAt);
            builder.HasIndex(x => x.AquariumId);
        }
    }
}
