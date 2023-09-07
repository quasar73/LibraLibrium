using LibraLibrium.Services.Profile.API.Application.Models;
using MediatR;

namespace LibraLibrium.Services.Profile.API.Application.Queries;

public record GetProfileQuery : IRequest<ProfileDTO>
{
    public string Identity { get; init; } = null!;

    public GetProfileQuery(string identity)
    {
        Identity = identity;
    }
}
