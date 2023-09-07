using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using LibraLibrium.Services.Profile.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace LibraLibrium.Services.Profile.Infrastructure.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly ProfileContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ProfileRepository(ProfileContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserProfile?> FindByIdentityAsync(string identity)
    {
        var profile = await _context.Profiles
            .Where(p => p.Identity == identity)
            .SingleOrDefaultAsync();

        return profile;
    }
}
