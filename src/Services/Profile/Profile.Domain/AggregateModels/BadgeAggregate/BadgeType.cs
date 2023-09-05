namespace LibraLibrium.Services.Profile.Domain.AggregateModels.BadgeAggregate;

public class BadgeType
    : Enumeration
{
    public static BadgeType Trader = new(1, nameof(Trader).ToLowerInvariant());
    public static BadgeType Developer = new(2, nameof(Developer).ToLowerInvariant());
    public static BadgeType Creator = new(3, nameof(Creator).ToLowerInvariant());

    public BadgeType(int id, string name) : base(id, name) { }

    public static IEnumerable<BadgeType> List() => new[] { Trader, Developer, Creator };

    public static BadgeType FromName(string name)
    {
        var state = List().SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (state == null)
        {
            throw new ProfileDomainException($"Possible values for BadgeType: {string.Join(",", List().Select(s => s.Name))}");
        }

        return state;
    }

    public static BadgeType From(int id)
    {
        var state = List().SingleOrDefault(s => s.Id == id);

        if (state == null)
        {
            throw new ProfileDomainException($"Possible values for BadgeType: {string.Join(",", List().Select(s => s.Name))}");
        }

        return state;
    }
}
