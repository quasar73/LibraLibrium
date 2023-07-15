namespace LibraLibrium.Services.Trading.Domain.AggregateModels.TradeAggregate;

public class Trade 
    : Entity, IAggregateRoot
{
    private readonly List<TradeEntry> _entries;
    public IReadOnlyCollection<TradeEntry> Entries => _entries;

    public DateTime CreatedAt { get; private set; }

    public DateTime ClosedAt { get; private set; }

    public string SenderId => _senderId;
    private string _senderId;

    public string ReceiverId => _receiverId;
    private string _receiverId;

    public bool ReceiverAccepted { get; private set; }
    public bool SenderAccepted { get; private set; }

    public Trade(DateTime createdAt, DateTime closedAt, string receiverId, string senderId)
    {
        _entries = new List<TradeEntry>();
        CreatedAt = createdAt;
        ClosedAt = closedAt;
        _receiverId = receiverId;
        _senderId = senderId;
        ReceiverAccepted = false;
        SenderAccepted = false;
    }

    public void AddTradeEntry(int bookId, int generation, DateTime createdAt, EntryType type)
    {
        if (_entries is not { Count: 0 })
        {
            var currentGeneration = _entries.Max(entry => entry.Generation);

            if (generation < currentGeneration)
            {
                throw new TradingDomainException($"The generation: {generation} is less then currentGeneration: {currentGeneration}");
            }
        }

        var entry = new TradeEntry(bookId, generation, createdAt, type);
        _entries.Add(entry);
    }
}
