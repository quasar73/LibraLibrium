namespace LibraLibrium.Services.Books.API.Infrastructure;

public class BooksContext : DbContext
{
    public DbSet<Book> Books { get; set; }

    public BooksContext(DbContextOptions<BooksContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
    }
}
