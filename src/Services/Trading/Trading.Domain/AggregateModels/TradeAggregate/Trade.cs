namespace LibraLibrium.Services.Trading.Domain.AggregateModels.TradeAggregate;

public class Trade 
    : Entity, IAggregateRoot
{
    private readonly List<TradeEntry> _entries;
    public IReadOnlyCollection<TradeEntry> Entries => _entries;

    public DateTime CreatedAt { get; private set; }

    public DateTime? ClosedAt { get; private set; }

    public string SenderId => _senderId;
    private string _senderId;

    public string ReceiverId => _receiverId;
    private string _receiverId;

    public bool AcceptedByReceiver { get; private set; }
    public bool AcceptedBySender { get; private set; }

    public bool GenerationClosed { get; private set; }

    public bool Closed { get; private set; }

    private Trade()
    {
        _entries = new List<TradeEntry>();
        AcceptedByReceiver = false;
        AcceptedBySender = false;
        GenerationClosed = false;
        _senderId = string.Empty;
        _receiverId = string.Empty;
    }

    public Trade(DateTime createdAt, string receiverId, string senderId)
    {
        if (!Guid.TryParse(receiverId, out Guid receiverResult))
        {
            throw new ArgumentException("Receiver Id must be the GUID.");
        }

        if (!Guid.TryParse(senderId, out Guid senderResult))
        {
            throw new ArgumentException("Sender Id must be the GUID.");
        }

        _entries = new List<TradeEntry>();
        _receiverId = receiverId;
        _senderId = senderId;
        CreatedAt = createdAt;
        ClosedAt = null;
        AcceptedByReceiver = false;
        AcceptedBySender = false;
        GenerationClosed = false;
    }

    public void AddTradeEntry(int bookId, int generation, string traderId, DateTime createdAt, EntryType type)
    {
        if (traderId == null || (traderId != SenderId && traderId != ReceiverId))
        {
            throw new TradingDomainException($"Trader with id: {traderId} is not able to do this trade");
        }

        if (_entries is not { Count: 0 })
        {
            var currentGeneration = _entries.Max(entry => entry.Generation);

            if (generation < currentGeneration)
            {
                throw new TradingDomainException($"The generation: {generation} is less then currentGeneration: {currentGeneration}");
            }

            if (_entries.Where(e => e.Generation == currentGeneration).Any(e => e.TraderId != traderId))
            {
                throw new TradingDomainException($"Trader with id: {traderId} is not able to add new entries in current generation");
            }
        }

        var entry = new TradeEntry(bookId, generation, traderId, createdAt, type);
        _entries.Add(entry);

        GenerationClosed = false;
    }

    public void CloseGeneration(string traderId)
    {
        if (traderId == null || (traderId != SenderId && traderId != ReceiverId))
        {
            throw new TradingDomainException($"Trader with id: {traderId} is not able to do this trade");
        }

        if (_entries is not { Count: 0 })
        {
            var currentGeneration = _entries.Max(entry => entry.Generation);

            if (_entries.Where(e => e.Generation == currentGeneration).Any(e => e.TraderId != traderId))
            {
                throw new TradingDomainException($"Trader with id: {traderId} is not able to close current generation");
            }
        }

        GenerationClosed = true;
    }
}
