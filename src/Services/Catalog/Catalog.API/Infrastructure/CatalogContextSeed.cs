using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace LibraLibrium.Services.Catalog.API.Infrastructure;

public class CatalogContextSeed
{
    public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(CatalogContextSeed));

        var contentRootPath = env.ContentRootPath;

        await policy.ExecuteAsync(async () =>
        {
            if (!context.Authors.Any())
            {
                await context.Authors.AddRangeAsync(GetAuthorsFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }

            if (!context.Publisher.Any())
            {
                await context.Publisher.AddRangeAsync(GetPublishersFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }

            if (!context.BookSamples.Any())
            {
                await context.BookSamples.AddRangeAsync(GetBookSamplesFromFile(contentRootPath, context, logger));
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<Author> GetAuthorsFromFile(string contentRootPath, ILogger<CatalogContextSeed> logger)
    {
        string csvFileAuthors = Path.Combine(contentRootPath, "Setup", "Authors.csv");

        if (!File.Exists(csvFileAuthors))
        {
            return Enumerable.Empty<Author>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "fullname", "originalname" };
            csvheaders = GetHeaders(csvFileAuthors, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<Author>();
        }

        return File.ReadAllLines(csvFileAuthors)
                                    .Skip(1)
                                    .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                    .SelectTry(column => CreateAuthor(column, csvheaders))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private Author CreateAuthor(string[] column, string[] headers)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        return new Author
        {
            FullName = column[Array.IndexOf(headers, "fullname")],
            OriginalName = column[Array.IndexOf(headers, "originalname")],
        };
    }

    private IEnumerable<Publisher> GetPublishersFromFile(string contentRootPath, ILogger<CatalogContextSeed> logger)
    {
        string csvFilePublishers = Path.Combine(contentRootPath, "Setup", "Publishers.csv");

        if (!File.Exists(csvFilePublishers))
        {
            return Enumerable.Empty<Publisher>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "name" };
            csvheaders = GetHeaders(csvFilePublishers, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<Publisher>();
        }

        return File.ReadAllLines(csvFilePublishers)
                                    .Skip(1)
                                    .SelectTry(x => CreatePublisher(x))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private Publisher CreatePublisher(string name)
    {
        name = name.Trim('"').Trim();

        return new Publisher
        {
            Name = name,
        };
    }

    private IEnumerable<BookSample> GetBookSamplesFromFile(string contentRootPath, CatalogContext context, ILogger<CatalogContextSeed> logger)
    {
        string csvFileBookSamples = Path.Combine(contentRootPath, "Setup", "BookSamples.csv");

        if (!File.Exists(csvFileBookSamples))
        {
            return Enumerable.Empty<BookSample>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "title", "description", "numberofpages", "authornames", "publishername" };
            csvheaders = GetHeaders(csvFileBookSamples, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<BookSample>();
        }

        var authorIdLookup = context.Authors.ToDictionary(x => x.FullName, x => x);
        var publisherIdLookup = context.Publisher.ToDictionary(x => x.Name, x => x.Id);

        return File.ReadAllLines(csvFileBookSamples)
                   .Skip(1)
                   .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                   .SelectTry(column => CreateBookSample(column, csvheaders, authorIdLookup, publisherIdLookup))
                   .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                   .Where(x => x != null);
    }

    private BookSample CreateBookSample(string[] column, string[] headers, Dictionary<string, Author> authorIdLookup, Dictionary<string, int> publisherIdLookup)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        string publisherName = column[Array.IndexOf(headers, "publishername")].Trim('"').Trim();
        if (!publisherIdLookup.ContainsKey(publisherName))
        {
            throw new Exception($"publisher name={publisherName} does not exist in publishers");
        }

        string[] authorNames = column[Array.IndexOf(headers, "authornames")].Trim('"').Trim().Split(',');
        if (!authorIdLookup.Keys.Any(k => authorNames.Contains(k)))
        {
            throw new Exception($"author name={publisherName} does not exist in authors");
        }

        string numberOfPagesString = column[Array.IndexOf(headers, "numberofpages")].Trim('"').Trim();
        if (!int.TryParse(numberOfPagesString, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numberOfPages))
        {
            throw new Exception($"numberOfPagesString={numberOfPagesString} is not a valid integer number");
        }

        var bookSample = new BookSample()
        {
            Title = column[Array.IndexOf(headers, "title")].Trim('"').Trim(),
            Description = column[Array.IndexOf(headers, "description")].Trim('"').Trim(),
            NumberOfPages = numberOfPages,
            PublisherId = publisherIdLookup[publisherName],
            Authors = authorIdLookup.Where(x => authorNames.Contains(x.Key)).Select(x => x.Value).ToList(),
        };

        return bookSample;
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

    private AsyncRetryPolicy CreatePolicy(ILogger<CatalogContextSeed> logger, string prefix, int retries = 3)
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
