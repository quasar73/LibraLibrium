namespace LibraLibrium.Services.Identity.API.Infrastructure;

public class ApplicationDbContextSeed
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

    public async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env, ILogger<ApplicationDbContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(ApplicationDbContextSeed));

        var contentRootPath = env.ContentRootPath;

        await policy.ExecuteAsync(async () =>
        {
            if (!context.Users.Any())
            {
                context.Users.AddRange(GetUsersFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<ApplicationUser> GetUsersFromFile(string contentRootPath, ILogger<ApplicationDbContextSeed> logger)
    {
        string csvFileUsers = Path.Combine(contentRootPath, "Setup", "Users.csv");

        if (!File.Exists(csvFileUsers))
        {
            return Enumerable.Empty<ApplicationUser>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = {
                    "id", "city", "country", "email", "name",
                    "username", "state", "securitynumber",
                    "normalizedemail", "normalizedusername", "password"
                };
            csvheaders = GetHeaders(requiredHeaders, csvFileUsers);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);

            return Enumerable.Empty<ApplicationUser>();
        }

        List<ApplicationUser> users = File.ReadAllLines(csvFileUsers)
                    .Skip(1) // skip header column
                    .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                    .SelectTry(column => CreateApplicationUser(column, csvheaders))
                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                    .Where(x => x != null)
                    .ToList();

        return users;
    }

    private ApplicationUser CreateApplicationUser(string[] column, string[] headers)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        var user = new ApplicationUser
        {
            Id = column[Array.IndexOf(headers, "id")].Trim('"').Trim(),
            City = column[Array.IndexOf(headers, "city")].Trim('"').Trim(),
            Country = column[Array.IndexOf(headers, "country")].Trim('"').Trim(),
            Email = column[Array.IndexOf(headers, "email")].Trim('"').Trim(),
            Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
            UserName = column[Array.IndexOf(headers, "username")].Trim('"').Trim(),
            State = column[Array.IndexOf(headers, "state")].Trim('"').Trim(),
            NormalizedEmail = column[Array.IndexOf(headers, "normalizedemail")].Trim('"').Trim(),
            NormalizedUserName = column[Array.IndexOf(headers, "normalizedusername")].Trim('"').Trim(),
            SecurityStamp = Guid.NewGuid().ToString("D"),
            PasswordHash = column[Array.IndexOf(headers, "password")].Trim('"').Trim(),
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

        return user;
    }

    static string[] GetHeaders(string[] requiredHeaders, string csvfile)
    {
        string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

        if (csvheaders.Count() != requiredHeaders.Count())
        {
            throw new Exception($"requiredHeader count '{requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
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

    private AsyncRetryPolicy CreatePolicy(ILogger<ApplicationDbContextSeed> logger, string prefix, int retries = 3)
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