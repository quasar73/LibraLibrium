namespace LibraLibrium.Services.Trading.Domain.AggregateModels.TradeAggregate;

public class EntryType
    : Enumeration
{
    public static EntryType Added = new EntryType(1, nameof(Added).ToLowerInvariant());
    public static EntryType Removed = new EntryType(2, nameof(Removed).ToLowerInvariant());

    public EntryType(int id, string name)
        : base(id, name)
    {
    }

    public static IEnumerable<EntryType> List() =>
        new[] { Added, Removed };

    public static EntryType FromName(string name)
    {
        var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state == null)
        {
            throw new TradingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
        }

        return state;
    }

    public static EntryType From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);

        if (state == null)
        {
            throw new TradingDomainException($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
        }

        return state;
    }
}
