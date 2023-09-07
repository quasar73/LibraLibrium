namespace LibraLibrium.Services.Profile.API.Application.Models;

public record ProfileDTO
{
    public string Name { get; init; } = null!;

    public string City { get; init; } = null!;

    public string State { get; init; } = null!;

    public string Country { get; init; } = null!;

    public float Rating { get; init; }
}
