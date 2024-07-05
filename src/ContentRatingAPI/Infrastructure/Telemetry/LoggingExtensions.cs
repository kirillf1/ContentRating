using Serilog.Events;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Sensitive;

namespace ContentRatingAPI.Infrastructure.Telemetry
{
    public static class LoggingExtensions
    {
        public static Logger CreateSerilogLogger()
        {
           
           return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .Enrich.WithSensitiveDataMasking(options => options.MaskProperties.AddRange(["email", "token", "accesstoken", "refreshtoken", "password"]))
            .CreateLogger();
        }
    }
}
