namespace LibraLibrium.Services.Catalog.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomDbContext(Configuration)
            .AddCustomHealthCheck(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });
    }
}

public static class CustomExtensionMethods
{
    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkNpgsql()
            .AddDbContext<CatalogContext>(options =>
            {
                options.UseNpgsql(configuration["ConnectionString"],
                                        npgsqlOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(typeof(CatalogContext).GetTypeInfo().Assembly.GetName().Name);
                                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                                        });
                options.UseInternalServiceProvider(services.BuildServiceProvider());
            });

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy())
            .AddNpgSql(
                configuration["ConnectionString"],
                name: "CatalogDb-check",
                tags: new string[] { "catalogsdb" });

        return services;
    }
}