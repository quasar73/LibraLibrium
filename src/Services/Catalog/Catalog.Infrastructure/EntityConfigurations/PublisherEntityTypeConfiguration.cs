namespace LibraLibrium.Services.Catalog.Infrastructure.EntityConfigurations;

public class PublisherEntityTypeConfiguration
    : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");

        builder.HasKey(x => x.Id);

        builder.Property(ci => ci.Id).IsRequired();
        builder.Property(ci => ci.Name).IsRequired();
    }
}
