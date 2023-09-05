using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using LibraLibrium.Services.Profile.Domain.AggregateModels.BadgeAggregate;

namespace LibraLibrium.Services.Profile.Infrastructure.EntityConfiguration;

public class BadgeTypeEntityTypeConfiguration
    : IEntityTypeConfiguration<BadgeType>
{
    public void Configure(EntityTypeBuilder<BadgeType> builder)
    {
        builder.ToTable("BadgeType", ProfileContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(o => o.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}