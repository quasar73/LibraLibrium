namespace LibraLibrium.Services.Catalog.Infrastructure.EntityConfigurations;

public class BookSampleEntityTypeConfiguration
    : IEntityTypeConfiguration<BookSample>
{
    public void Configure(EntityTypeBuilder<BookSample> builder)
    {
        builder.ToTable("BookSamples");

        builder.HasKey(x => x.Id);

        builder.Property(ci => ci.Id).IsRequired();
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Description).IsRequired();

        builder.HasOne(x => x.Publisher)
            .WithMany()
            .HasForeignKey(x => x.PublisherId);

        builder.HasMany(x => x.Authors)
            .WithMany(x => x.BookSamples);
    }
}
