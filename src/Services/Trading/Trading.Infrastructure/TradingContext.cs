namespace LibraLibrium.Services.Trading.Infrastructure;

public class TradingContext : DbContext
{
    public const string DEFAULT_SCHEMA = "trading";
    public DbSet<Trade> Trades { get; set; }
    public DbSet<TradeEntry> TradeEntries { get; set; }

    public TradingContext(DbContextOptions<TradingContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TradeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TradeEntryEntityTypeConfiguration());
    }
}
