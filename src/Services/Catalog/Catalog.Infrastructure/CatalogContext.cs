namespace LibraLibrium.Services.Catalog.Infrastructure;

public class CatalogContext : DbContext
{
    public DbSet<BookSample> BookSamples { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publisher { get; set; }

    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
    {  
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AuthorEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BookSampleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PublisherEntityTypeConfiguration());
    }
}
