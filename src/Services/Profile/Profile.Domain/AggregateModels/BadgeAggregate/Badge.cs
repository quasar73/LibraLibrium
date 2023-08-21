namespace LibraLibrium.Services.Profile.Domain.AggregateModels.BadgeAggregate;

public class Badge
    : Entity
{
    public BadgeType Type { get; private set; } = null!;
    private int _typeId;

    public string Title { get; private set; }

    public string Description { get; private set; }

    public int Level { get; private set; }

    private Badge() 
    {
        _typeId = 0;
        Title = string.Empty;
        Description = string.Empty;
        Level = 0;
    }

    public Badge(string title, string description, int level, BadgeType type)
    {
        Title = title;
        Description = description;
        Level = level;
        _typeId = type.Id;
    }
}
