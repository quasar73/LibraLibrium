﻿namespace LibraLibrium.Services.Trading.Domain.SeedWork;

public class TradeEntry : Entity
{
    public EntryType Type { get; private set; }
    private int _typeId;

    public int BookId => _bookId;
    private int _bookId;

    public int Generation { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string TraderId => _traderId;
    private string _traderId;

    protected TradeEntry() { }

    public TradeEntry(int bookId, int generation, string traderId, DateTime createdAt, EntryType type)
    {
        _bookId = bookId;
        Generation = generation;
        CreatedAt = createdAt;
        _typeId = type.Id;
        _traderId = traderId;
    }
}
