using Npgsql;
using Polly.Retry;
using Polly;
using System.Globalization;
using System.Text.RegularExpressions;
using LibraLibrium.Services.Profile.API.Extensions;
using LibraLibrium.Services.Profile.Domain.AggregateModels.BadgeAggregate;
using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using LibraLibrium.Services.Profile.Infrastructure;
using Serilog;

namespace LibraLibrium.Services.Profile.API.Infrastructure;

public class ProfileContextSeed
{
    public async Task SeedAsync(ProfileContext context, IWebHostEnvironment env, ILogger<ProfileContextSeed> logger)
    {
        var policy = CreatePolicy(logger, nameof(ProfileContextSeed));

        var contentRootPath = env.ContentRootPath;

        await policy.ExecuteAsync(async () =>
        {
            if (!context.BadgeTypes.Any())
            {
                await context.BadgeTypes.AddRangeAsync(GetBadgeTypesFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }

            if (!context.Badges.Any())
            {
                await context.Badges.AddRangeAsync(GetBadgesFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }

            if (!context.Profiles.Any())
            {
                await context.Profiles.AddRangeAsync(GetProfilesFromFile(contentRootPath, logger));
                await context.SaveChangesAsync();
            }
        });
    }

    private IEnumerable<BadgeType> GetBadgeTypesFromFile(string contentRootPath, ILogger<ProfileContextSeed> logger)
    {
        string csvFileBadgeType = Path.Combine(contentRootPath, "Setup", "BadgeType.csv");

        if (!File.Exists(csvFileBadgeType))
        {
            return Enumerable.Empty<BadgeType>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "badgetype" };
            csvheaders = GetHeaders(csvFileBadgeType, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<BadgeType>();
        }

        int id = 1;
        return File.ReadAllLines(csvFileBadgeType)
                                    .Skip(1) // skip header row
                                    .SelectTry(x => CreateOrderStatus(x, ref id))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private BadgeType CreateOrderStatus(string value, ref int id)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception("Orderstatus is null or empty");
        }

        return new BadgeType(id++, value.Trim('"').Trim().ToLowerInvariant());
    }


    private IEnumerable<Badge> GetBadgesFromFile(string contentRootPath, ILogger<ProfileContextSeed> logger)
    {
        string csvFileBadge = Path.Combine(contentRootPath, "Setup", "Badge.csv");

        if (!File.Exists(csvFileBadge))
        {
            return Enumerable.Empty<Badge>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "type_id", "title", "description", "level" };
            csvheaders = GetHeaders(csvFileBadge, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<Badge>();
        }

        return File.ReadAllLines(csvFileBadge)
                                    .Skip(1)
                                    .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                    .SelectTry(column => CreateBadge(column, csvheaders))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private Badge CreateBadge(string[] column, string[] headers)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        string typeIdStringified = column[Array.IndexOf(headers, "type_id")].Trim('"').Trim();
        if (!int.TryParse(typeIdStringified, NumberStyles.Integer, CultureInfo.InvariantCulture, out int typeId))
        {
            throw new Exception($"typeIdStringified={typeIdStringified} is not a valid integer number");
        }

        string levelStringified = column[Array.IndexOf(headers, "level")].Trim('"').Trim();
        if (!int.TryParse(levelStringified, NumberStyles.Integer, CultureInfo.InvariantCulture, out int level))
        {
            throw new Exception($"levelStringified={levelStringified} is not a valid integer number");
        }

        string description = column[Array.IndexOf(headers, "description")];
        string title = column[Array.IndexOf(headers, "title")];

        return new Badge(title, description, level, BadgeType.From(typeId));
    }

    private IEnumerable<UserProfile> GetProfilesFromFile(string contentRootPath, ILogger<ProfileContextSeed> logger)
    {
        string csvFileUserProfile = Path.Combine(contentRootPath, "Setup", "Profile.csv");

        if (!File.Exists(csvFileUserProfile))
        {
            return Enumerable.Empty<UserProfile>();
        }

        string[] csvheaders;
        try
        {
            string[] requiredHeaders = { "identity", "name", "city", "state", "country" };
            csvheaders = GetHeaders(csvFileUserProfile, requiredHeaders);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            return Enumerable.Empty<UserProfile>();
        }

        return File.ReadAllLines(csvFileUserProfile)
                                    .Skip(1)
                                    .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                    .SelectTry(column => CreateProfile(column, csvheaders))
                                    .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                    .Where(x => x != null);
    }

    private UserProfile CreateProfile(string[] column, string[] headers)
    {
        if (column.Count() != headers.Count())
        {
            throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
        }

        string identity = column[Array.IndexOf(headers, "identity")];
        if (!Guid.TryParse(identity, out Guid guidIdentity))
        {
            throw new Exception($"identity={identity} is not a valid GUID");
        }

        string name = column[Array.IndexOf(headers, "name")];
        string city = column[Array.IndexOf(headers, "city")];
        string state = column[Array.IndexOf(headers, "state")];
        string country = column[Array.IndexOf(headers, "country")];

        return new UserProfile(identity, name, city, state, country);
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

    private AsyncRetryPolicy CreatePolicy(ILogger<ProfileContextSeed> logger, string prefix, int retries = 3)
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
