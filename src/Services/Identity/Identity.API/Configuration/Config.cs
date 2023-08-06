namespace LibraLibrium.Services.Identity.API.Configuration;

public class Config
{
    public static IEnumerable<ApiResource> GetApis()
    {
        return new List<ApiResource>
        {
            new ApiResource("books", "Books Service"),
            new ApiResource("catalog", "Catalog Service"),
            new ApiResource("trading", "Trading Service"),
        };
    }

    public static IEnumerable<ApiScope> GetScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope("books", "Books Service"),
            new ApiScope("catalog", "Catalog Service"),
            new ApiScope("trading", "Trading Service"),
        };
    }

    public static IEnumerable<IdentityResource> GetResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

    public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "spa",
                ClientName = "LibraLibrium SPA OpenId Client",
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RedirectUris =           { $"{clientsUrl["Spa"]}/" },
                RequireConsent = false,
                PostLogoutRedirectUris = { $"{clientsUrl["Spa"]}/" },
                AllowedCorsOrigins =     { $"{clientsUrl["Spa"]}" },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "books",
                    "catalog",
                    "trading",
                },
            },
        };
    }
}