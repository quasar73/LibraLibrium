namespace LibraLibrium.Services.Trading.Domain.Exceptions;

/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class TradingDomainException : Exception
{
    public TradingDomainException()
    { }

    public TradingDomainException(string message)
        : base(message)
    { }

    public TradingDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}