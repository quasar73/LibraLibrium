using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Books.API.Infrastructure.EntityConfigurations;

public class BookEntityTypeConfiguration
    : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.OwnerIdentity).IsRequired();
    }
}
