namespace LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;

public class Profile
    : Entity, IAggregateRoot
{
    public string Name { get; private set; }

    public string City { get; private set; }

    public string State { get; private set; }

    public string Country { get; private set; }

    public float Rating { get; private set; }

    private readonly List<Badge> _badges;
    public IReadOnlyCollection<Badge> Badges => _badges;

    private Profile()
    {
        Rating = 0.5f;
        Name = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Country = string.Empty;
        _badges = new List<Badge>();
    }

    public Profile(string name, string city, string state, string country)
    {
        Rating = 0.5f;
        Name = name;
        City = city;
        State = state;
        Country = country;
        _badges = new List<Badge>();
    }

    public void AwardTheBadge(Badge badge)
    {
        var sameOrBetterBadgeIsExist = _badges.Any(b => b.Type.Equals(badge.Type) && b.Level >= badge.Level);

        if (sameOrBetterBadgeIsExist)
        {
            throw new ProfileDomainException($"Badge with id: {badge.Id} already owned by the user. User id: {Id}");
        }

        _badges.Add(badge);
    }

    public void SetRating(float rating)
    {
        if (rating > 1 || rating < 0)
        {
            throw new ProfileDomainException($"Rating can be only between 0 and 1. The rating now is {rating}");
        }

        Rating = rating;
    }
}
