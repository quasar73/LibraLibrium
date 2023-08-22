namespace LibraLibrium.Services.Trading.Infrastructure.Repositories;

public class TradeRepository : ITradeRepository
{
    private readonly TradingContext _context;
    public IUnitOfWork UnitOfWork => _context;

    public TradeRepository(TradingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Trade Add(Trade trade)
    {
        return _context.Trades.Add(trade).Entity;
    }

    public void Update(Trade trade)
    {
        _context.Trades.Update(trade);
    }

    public async Task<Trade> GetAsync(int tradeId)
    {
        var trade = await _context.Trades
            .Where(t => t.Id == tradeId)
            .SingleOrDefaultAsync();

        return trade;
    }
}
