using LibraLibrium.Services.Profile.Domain.AggregateModels.BadgeAggregate;
using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using LibraLibrium.Services.Profile.Domain.SeedWork;
using LibraLibrium.Services.Profile.Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace LibraLibrium.Services.Profile.Infrastructure;

public class ProfileContext : DbContext, IUnitOfWork
{
    public const string DEFAULT_SCHEMA = "profile";
    public DbSet<UserProfile> Profiles { get; set; }
    public DbSet<Badge> Badges { get; set; }
    public DbSet<BadgeType> BadgeTypes { get; set; }

    public ProfileContext(DbContextOptions<ProfileContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProfileEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BadgeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BadgeTypeEntityTypeConfiguration());
    }

    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
