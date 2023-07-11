namespace LibraLibrium.Services.Identity.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCustomHealthCheck(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder
            .AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }
}

