namespace LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;

public interface IProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> FindByIdentityAsync(string identity);
}
