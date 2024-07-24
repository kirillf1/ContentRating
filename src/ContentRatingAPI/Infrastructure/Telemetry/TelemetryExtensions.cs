using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ContentRatingAPI.Infrastructure.Telemetry
{
    public static class TelemetryExtensions
    {

        public static IServiceCollection AddTelemetry(this IHostApplicationBuilder builder)
        {
            var otel = builder.Services.AddOpenTelemetry();
            var configuration = builder.Configuration;

            var tracingAddress = configuration["Telemetry:Tracing:CollectorAddress"];

            if (!string.IsNullOrEmpty(tracingAddress))
            {
                otel.WithTracing(builder =>
                {
                    builder.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                .AddService(serviceName: configuration["Application:Name"] ?? "ContentRating", autoGenerateServiceInstanceId: false,
                                        serviceInstanceId: configuration["Application:Instance"] ?? "ContentRatingProd"))
                        .AddAspNetCoreInstrumentation()
                        .AddOtlpExporter(opts =>
                        {
                            opts.Endpoint =
                                new Uri(configuration["Telemetry:Tracing:CollectorAddress"]);
                        });
                });
            }

            var metricsAddress = configuration["Telemetry:Metrics:CollectorAddress"];

            if (!string.IsNullOrEmpty(metricsAddress))
            {
                otel.WithMetrics(opts => opts
               .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                .AddService(serviceName: configuration["Application:Name"] ?? "ContentRating", autoGenerateServiceInstanceId: false,
                                        serviceInstanceId: configuration["Application:Instance"] ?? "ContentRatingProd"))
                .AddProcessInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(opts =>
                {
                    opts.Endpoint =
                        new Uri(configuration["Telemetry:Metrics:CollectorAddress"]);
                })
            );
            }
            return builder.Services;
        }
    }
}
