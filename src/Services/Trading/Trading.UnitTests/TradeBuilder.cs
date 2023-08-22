namespace LibraLibrium.Services.Trading.UnitTests;

public class TradeBuilder
{
    private readonly Trade trade;

    public TradeBuilder(DateTime createdAt, string receiverId, string senderId)
    {
        trade = new Trade(createdAt, receiverId, senderId);
    }

    public TradeBuilder AddEntry(int bookId, int generation, string traderId, DateTime createdAt, EntryType type)
    {
        trade.AddTradeEntry(bookId, generation, traderId, createdAt, type);

        return this;
    }

    public Trade Build()
    {
        return trade;
    }
}
