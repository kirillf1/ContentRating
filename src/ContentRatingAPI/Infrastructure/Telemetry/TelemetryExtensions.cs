using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ContentRatingAPI.Infrastructure.Telemetry
{
    public static class TelemetryExtensions
    {

        public static IServiceCollection AddTelemetry(this IHostApplicationBuilder builder)
        {
            var otel = builder.Services.AddOpenTelemetry();
            var configuration = builder.Configuration;
            if (bool.TryParse(configuration["Telemetry:Tracing:enabled"], out var isTracingEnabled) && isTracingEnabled)
            {
                otel.WithTracing(builder =>
                {
                    builder.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                .AddService(serviceName: configuration["AppName"] ?? "ContentRating", autoGenerateServiceInstanceId: false,
                                        serviceInstanceId: configuration["Instance"] ?? "ContentRatingProd"))
                        .AddAspNetCoreInstrumentation()
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint =
                                new Uri(configuration["Telemetry:Tracing:collector_address"]);
                        });
                });
            }
            if (bool.TryParse(configuration["Telemetry:Metrics:enabled"], out var isMetricsEnabled) && isMetricsEnabled)
            {
                otel.WithMetrics(opts => opts
               .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                .AddService(serviceName: configuration["AppName"] ?? "ContentRating", autoGenerateServiceInstanceId: false,
                                        serviceInstanceId: configuration["Instance"] ?? "ContentRatingProd"))
                .AddProcessInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint =
                        new Uri(configuration["Telemetry:Metrics:collector_address"]);
                })
            );
            }
            return builder.Services;
        }
    }
}
