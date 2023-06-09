﻿using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace WebApi.Setups;

public static class LoggingSetup
{
    public static IHostBuilder UseLoggingSetup(this IHostBuilder host, IConfiguration configuration)
    {
        host.UseSerilog((_, services, lc) =>
        {
            lc.ConfigureBaseLogging(configuration, services);
        });

        return host;
    }

}

public static class LoggerExtension
{
    public static LoggerConfiguration ConfigureBaseLogging(
        this LoggerConfiguration loggerConfiguration, IConfiguration configuration, IServiceProvider services
    )
    {
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Destructure.AsScalar<JObject>()
            .Destructure.AsScalar<JArray>()
            .WriteTo.Async(a =>
            {
                a.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code);
            })
            .Enrich.FromLogContext();

        return loggerConfiguration;
    }
}