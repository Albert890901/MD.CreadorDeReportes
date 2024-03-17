using MD.CreadorDeReportes;
using MD.CreadorDeReportes.Interfaces;
using MD.CreadorDeReportes.Modelos;
using MD.CreadorDeReportes.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builtConfig = new ConfigurationBuilder()
                                  .AddJsonFile("appsettings.json")
                                  .AddCommandLine(args)
                                  .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        LogLevel logLevel = builtConfig.GetSection("Logging").GetSection("LogLevel").GetValue<LogLevel>("Default");

        services.AddHostedService<Worker>()
        .AddLogging(builder =>
                    builder.ClearProviders()
                    .AddConsole()
                    .AddLog4Net("log4net.config")
                    .SetMinimumLevel(logLevel)
                )

        .AddDbContext<ReportesContext>(op => op.UseNpgsql(builtConfig.GetConnectionString("FabConnection")))

        .AddScoped<IDBAcciones, DBAcciones>()
        .AddScoped<IRabbitMQConsumerService, RabbitMQConsumerService>()
        .AddScoped<IReporte, Reporte>()
        .AddScoped<IEmail, Email>();
    })
    
    .Build();

host.Run();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await host.RunAsync();