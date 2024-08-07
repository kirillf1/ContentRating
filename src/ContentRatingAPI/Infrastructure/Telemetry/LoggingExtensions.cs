﻿using Serilog.Events;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Sensitive;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Enrichers.Span;

namespace ContentRatingAPI.Infrastructure.Telemetry
{
    public static class LoggingExtensions
    {
        public static Logger CreateSerilogLogger(IConfiguration configuration, string environment)
        {
            var isDevelopment = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);

            var logBuilder = new LoggerConfiguration()
              .Enrich.WithSpan()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
              .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
              .Enrich.FromLogContext();

            if (isDevelopment)
            {
                logBuilder = logBuilder.WriteTo.Async(c => c.Console());
            }
            else
            {
                logBuilder = logBuilder.WriteTo.Async(c=> c.Console(restrictedToMinimumLevel: LogEventLevel.Information))
                    .Enrich.WithSensitiveDataMasking(options => options.MaskProperties.AddRange(["email", "token", "accesstoken", "refreshtoken", "password"]));
            }
            var lokiAddress = configuration["Telemetry:Logging:LokiAddress"];
            if (!string.IsNullOrEmpty(lokiAddress))
            {
                var instanceName = configuration["Application:Instance"] ?? typeof(Program).Assembly.GetName().Name!;
                logBuilder = logBuilder.WriteTo.GrafanaLoki(
                    lokiAddress,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    labels: [new LokiLabel { Key = "app", Value = instanceName }],
                    propertiesAsLabels: ["level"]
                   );
            }
            return logBuilder.CreateLogger();
        }
    }
}
