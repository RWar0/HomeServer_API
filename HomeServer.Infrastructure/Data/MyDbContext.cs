using Microsoft.EntityFrameworkCore;
using HomeServer.Domain.Models;
using HomeServer.Domain.Base;
using System.Linq.Expressions;

namespace HomeServer.Infrastructure.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        #region Sets of models
        public required DbSet<User> Users { get; set; }
        public required DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public required DbSet<StoredFile> StoredFiles { get; set; }
        public required DbSet<Aquarium> Aquariums { get; set; }
        public required DbSet<WaterChange> WaterChanges { get; set; }
        public required DbSet<ParameterCheck> ParameterChecks { get; set; }
        public required DbSet<Vehicle> Vehicles { get; set; }
        public required DbSet<VehicleFueling> VehicleFuelings { get; set; }
        #endregion


        #region Overrided Methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(BuildNotDeletedQueryFilter(entityType.ClrType));
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries<BaseModel>();

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Modified;
                    entityEntry.Entity.UpdatedAt = DateTime.UtcNow;
                    entityEntry.Entity.DeletedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        #endregion


        #region Helpers
        private static LambdaExpression BuildNotDeletedQueryFilter(Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");

            var deletedAt = Expression.Property(parameter, nameof(BaseModel.DeletedAt));

            var nullConstant = Expression.Constant(null, typeof(DateTime?));

            var body = Expression.Equal(deletedAt, nullConstant);

            return Expression.Lambda(body, parameter);
        }
        #endregion
    }
}
