﻿namespace LibraLibrium.Services.Trading.Domain.AggregateModels.TradeAggregate;

public class EntryType
    : Enumeration
{
    public static EntryType Added = new(1, nameof(Added).ToLowerInvariant());
    public static EntryType Removed = new(2, nameof(Removed).ToLowerInvariant());

    public EntryType(int id, string name): base(id, name) { }

    public static IEnumerable<EntryType> List() => new[] { Added, Removed };

    public static EntryType FromName(string name)
    {
        var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state == null)
        {
            throw new TradingDomainException($"Possible values for EntryType: {string.Join(",", List().Select(s => s.Name))}");
        }

        return state;
    }

    public static EntryType From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);

        if (state == null)
        {
            throw new TradingDomainException($"Possible values for EntryType: {string.Join(",", List().Select(s => s.Name))}");
        }

        return state;
    }
}
