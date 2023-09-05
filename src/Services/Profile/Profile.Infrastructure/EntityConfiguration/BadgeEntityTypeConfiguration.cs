using LibraLibrium.Services.Profile.Domain.AggregateModels.BadgeAggregate;
using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Profile.Infrastructure.EntityConfiguration;

public class BadgeEntityTypeConfiguration
    : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.ToTable("Badges", ProfileContext.DEFAULT_SCHEMA);

        builder.HasKey(b => b.Id);

        builder.Ignore(b => b.DomainEvents);

        builder.Property(b => b.Id)
            .UseHiLo("badgeseq", ProfileContext.DEFAULT_SCHEMA);

        builder.Property(b => b.Title)
            .HasDefaultValue("[Badge Name]")
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasDefaultValue("[Badge Description]")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(b => b.Level)
            .HasDefaultValue(1)
            .IsRequired();

        builder
           .Property<int>("_typeId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("TypeId")
           .IsRequired(true);

        builder.HasOne(b => b.Type)
            .WithMany()
            .HasForeignKey("_typeId");

        builder
            .HasMany<UserProfile>()
            .WithMany(u => u.Badges)
            .UsingEntity("ProfileBadges");
    }
}
