namespace LibraLibrium.Services.Catalog.Infrastructure.EntityConfigurations;

public class AuthorEntityTypeConfiguration
    : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");

        builder.HasKey(x => x.Id);

        builder.Property(ci => ci.Id).IsRequired();
        builder.Property(ci => ci.OriginalName).IsRequired();
        builder.Property(ci => ci.FullName).IsRequired();
    }
}
