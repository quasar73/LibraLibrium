namespace LibraLibrium.Services.Trading.Domain.AggregateModels.TradeAggregate;

public interface ITradeRepository : IRepository<Trade>
{
    Trade Add(Trade trade);

    void Update(Trade trade);

    Task<Trade> GetAsync(int tradeId);
}
