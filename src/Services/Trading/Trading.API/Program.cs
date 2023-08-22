namespace LibraLibrium.Services.Trading.API;

public class Program
{
    public static string Namespace = typeof(Startup).Namespace!;
    public static string AppName = Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

    public static int Main(string[] args)
    {
        var configuration = GetConfiguration();

        Log.Logger = CreateSerilogLogger(configuration);

        try
        {
            Log.Information("Configuring web host ({ApplicationContext})...", Program.AppName);
            var host = CreateHostBuilder(configuration, args);

            Log.Information("Applying migrations ({ApplicationContext})...", Program.AppName);
            host.MigrateDbContext<TradingContext>((context, services) =>
            {
                var env = services.GetService<IWebHostEnvironment>()!;
                var logger = services.GetService<ILogger<TradingContextSeed>>()!;

                new TradingContextSeed().SeedAsync(context, env, logger).Wait();
            });

            Log.Information("Starting web host ({ApplicationContext})...", Program.AppName);
            host.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", Program.AppName);
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHost CreateHostBuilder(IConfiguration configuration, string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults((web) =>
            {
                web.CaptureStartupErrors(false)
                    .UseUrls()
                    .ConfigureKestrel(options =>
                    {
                        var port = configuration.GetValue("PORT", 80);
                        options.Listen(IPAddress.Any, port, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                    })
                    .UseStartup<Startup>();
            })
            .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
            .UseSerilog()
            .Build();
    }


    private static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;
        var seqServerUrl = configuration["Serilog:SeqServerUrl"];

        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("ApplicationContext", Program.AppName)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
            .Enrich.WithProperty("Environment", environment)
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
}