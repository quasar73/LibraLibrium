namespace LibraLibrium.Services.Trading.Infrastructure;

public class TradingContext : DbContext, IUnitOfWork
{
    public const string DEFAULT_SCHEMA = "trading";
    public DbSet<Trade> Trades { get; set; }
    public DbSet<TradeEntry> TradeEntries { get; set; }
    public DbSet<Book> Books { get; set; }

    public TradingContext(DbContextOptions<TradingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TradeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TradeEntryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
    }

    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
