using LibraLibrium.Services.Profile.API.Application.Models;
using LibraLibrium.Services.Profile.Infrastructure.Exceptions;
using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using MediatR;

namespace LibraLibrium.Services.Profile.API.Application.Queries;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDTO>
{
    private readonly IProfileRepository _repository;

    public GetProfileQueryHandler(IProfileRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<ProfileDTO> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _repository.FindByIdentityAsync(request.Identity);

        if (profile is null)
        {
            throw new NotFoundException($"Profile with parameter {nameof(request.Identity)}='{request.Identity}' was not found.");
        }

        var dto = new ProfileDTO
        {
            City = profile.City,
            State = profile.State,
            Country = profile.Country,
            Name = profile.Name,
            Rating = profile.Rating,
        };

        return dto;
    }
}
