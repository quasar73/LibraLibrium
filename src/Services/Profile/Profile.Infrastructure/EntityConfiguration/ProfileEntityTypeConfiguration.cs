using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Profile.Infrastructure.EntityConfiguration;

public class ProfileEntityTypeConfiguration
    : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("Profiles", ProfileContext.DEFAULT_SCHEMA);

        builder.HasKey(p => p.Id);

        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.Id)
            .UseHiLo("profileseq", ProfileContext.DEFAULT_SCHEMA);

        builder.Property(p => p.Identity)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(p => p.City)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.State)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(p => p.Country)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(p => p.Rating)
            .HasDefaultValue(0.5)
            .IsRequired();

        var navigation = builder.Metadata.FindNavigation(nameof(UserProfile.Badges));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
