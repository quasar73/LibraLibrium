using LibraLibrium.Services.Trading.Domain.AggregateModels.BookAggregate;
using Polly.Retry;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LibraLibrium.Services.Trading.API.Infrastructure;

public class TradingContextSeed
{
    public async Task SeedAsync(TradingContext context, IWebHostEnvironment env, ILogger<TradingContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(TradingContextSeed));

        var contentRootPath = env.ContentRootPath;

        await policy.ExecuteAsync(async () =>
        {
            if (!context.Books.Any())
            {
                await context.Books.AddRangeAsync(GetBooksFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<Book> GetBooksFromFile(string contentRootPath, ILogger<TradingContextSeed> logger)
    {
        string csvFileBooks = Path.Combine(contentRootPath, "Setup", "Books.csv");

        if (!File.Exists(csvFileBooks))
        {
            return Enumerable.Empty<Book>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "bookid", "ownerid" };
            csvheaders = GetHeaders(csvFileBooks, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<Book>();
        }

        return File.ReadAllLines(csvFileBooks)
                                    .Skip(1)
                                    .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                    .SelectTry(column => CreateBook(column, csvheaders))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private Book CreateBook(string[] column, string[] headers)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        string bookIdStringified = column[Array.IndexOf(headers, "bookid")].Trim('"').Trim();
        if (!int.TryParse(bookIdStringified, NumberStyles.Integer, CultureInfo.InvariantCulture, out int bookId))
        {
            throw new Exception($"numberOfPagesString={bookIdStringified} is not a valid integer number");
        }

        string ownerId = column[Array.IndexOf(headers, "ownerid")];

        return new Book(ownerId, bookId);
    }

    private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
    {
        string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() < requiredHeaders.Count())
        {
            throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
        }

        if (optionalHeaders != null)
        {
            if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
            {
                throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
            }
        }

        foreach (var requiredHeader in requiredHeaders)
        {
            if (!csvheaders.Contains(requiredHeader))
            {
                throw new Exception($"does not contain required header '{requiredHeader}'");
            }
        }

        return csvheaders;
    }

    private AsyncRetryPolicy CreatePolicy(ILogger<TradingContextSeed> logger, string prefix, int retries = 3)
    {
        return Policy.Handle<NpgsqlException>().
            WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}
