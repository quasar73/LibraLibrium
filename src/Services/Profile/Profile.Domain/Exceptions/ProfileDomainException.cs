namespace LibraLibrium.Services.Profile.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class ProfileDomainException : Exception
{
    public ProfileDomainException()
    { }

    public ProfileDomainException(string message)
        : base(message)
    { }

    public ProfileDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}