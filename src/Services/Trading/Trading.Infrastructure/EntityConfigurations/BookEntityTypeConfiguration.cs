using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Trading.Infrastructure.EntityConfigurations;

public class BookEntityTypeConfiguration
    : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books", TradingContext.DEFAULT_SCHEMA);

        builder.HasKey(b => b.BookId);

        builder.Ignore(b => b.DomainEvents);
        builder.Ignore(b => b.Id);

        builder
           .Property<string>("_ownerId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("OwnerId")
           .IsRequired(true);
    }
}
