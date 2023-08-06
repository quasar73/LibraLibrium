namespace LibraLibrium.Services.Identity.API.Infrastructure;

public class ConfigurationDbContextSeed
{
    public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
    {
        var clientUrls = new Dictionary<string, string>
        {
            { "Spa", configuration.GetValue<string>("SpaClient") }
        };

        if (!context.ApiScopes.Any())
        {
            foreach (var scope in Config.GetScopes())
            {
                context.ApiScopes.Add(scope.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.IdentityResources.Any())
        {
            foreach (var resource in Config.GetResources())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            await context.SaveChangesAsync();
        }

        if (!context.ApiResources.Any())
        {
            foreach (var api in Config.GetApis())
            {
                context.ApiResources.Add(api.ToEntity());
            }

            await context.SaveChangesAsync();
        }

        if (!context.Clients.Any())
        {
            foreach (var client in Config.GetClients(clientUrls))
            {
                var client1 = client.ToEntity();
                context.Clients.Add(client1);
            }
            await context.SaveChangesAsync();
        }
    }
}