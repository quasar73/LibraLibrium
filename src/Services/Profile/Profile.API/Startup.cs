using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using LibraLibrium.Services.Profile.API.Application.Behaviors;
using LibraLibrium.Services.Profile.API.Infrastructure.Filters;
using LibraLibrium.Services.Profile.Domain.AggregateModels.ProfileAggregate;
using LibraLibrium.Services.Profile.Infrastructure;
using LibraLibrium.Services.Profile.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace LibraLibrium.Services.Profile.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCQRS()
            .AddCustomMvc()
            .AddCustomDbContext(Configuration)
            .AddCustomHealthCheck(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();

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
    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        })
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }

    public static IServiceCollection AddCQRS(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        services.AddScoped<IProfileRepository, ProfileRepository>();

        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkNpgsql()
            .AddDbContext<ProfileContext>(options =>
            {
                options.UseNpgsql(configuration["ConnectionString"],
                                        npgsqlOptionsAction: sqlOptions =>
                                        {
                                            sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
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
                configuration["ConnectionString"]!,
                name: "ProfileDb-check",
                tags: new string[] { "profiledb" });

        return services;
    }
}