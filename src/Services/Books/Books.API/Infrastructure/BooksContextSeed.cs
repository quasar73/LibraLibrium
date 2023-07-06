namespace LibraLibrium.Services.Books.API.Infrastructure;

public class BooksContextSeed
{
    public async Task SeedAsync(BooksContext context, IWebHostEnvironment env, ILogger<BooksContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(BooksContextSeed));

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

    private IEnumerable<Book> GetBooksFromFile(string contentRootPath, ILogger<BooksContextSeed> logger)
    {
        string csvFileAuthors = Path.Combine(contentRootPath, "Setup", "Books.csv");

        if (!File.Exists(csvFileAuthors))
        {
            return Enumerable.Empty<Book>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "title", "description", "numberofpages", "booksampleid", "owneridentity" };
            csvheaders = GetHeaders(csvFileAuthors, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<Book>();
        }

        return File.ReadAllLines(csvFileAuthors)
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

        string numberOfPagesString = column[Array.IndexOf(headers, "numberofpages")].Trim('"').Trim();
        if (!int.TryParse(numberOfPagesString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numberOfPages))
        {
            throw new Exception($"numberOfPagesString={numberOfPagesString} is not a valid integer number");
        }

        string bookSampleIdString = column[Array.IndexOf(headers, "booksampleid")];
        if (!int.TryParse(bookSampleIdString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int bookSampleId))
        {
            throw new Exception($"bookSampleIdString={numberOfPagesString} is not a valid integer number");
        }

        return new Book
        {
            Title = column[Array.IndexOf(headers, "title")],
            Description = column[Array.IndexOf(headers, "description")],
            NumberOfPages = numberOfPages,
            OwnerIdentity = column[Array.IndexOf(headers, "owneridentity")],
            BookSampleId = bookSampleId == -1 ? null : bookSampleId,
        };
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

    private AsyncRetryPolicy CreatePolicy(ILogger<BooksContextSeed> logger, string prefix, int retries = 3)
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
