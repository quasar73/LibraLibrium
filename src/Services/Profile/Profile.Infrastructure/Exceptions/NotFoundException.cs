namespace LibraLibrium.Services.Profile.Infrastructure.Exceptions;

/// <summary>
/// Exception type for cases when the object is not found
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException()
    { }

    public NotFoundException(string message)
        : base(message)
    { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
