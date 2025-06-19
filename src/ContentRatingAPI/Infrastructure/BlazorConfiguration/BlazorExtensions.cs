using System.Text.Json;
using Microsoft.AspNetCore.Localization;

namespace ContentRatingAPI.Infrastructure.BlazorConfiguration;

public static class BlazorExtensions
{
    public static WebApplicationBuilder AddBlazorWebAssembly(this WebApplicationBuilder builder)
    {
        // Blazor WebAssembly
        builder.Services.AddRazorPages();
        builder
            .Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddInteractiveServerComponents();

        // Локализация
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "en-US", "ru-RU" };
            options
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        // CORS для Blazor
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        return builder;
    }

    public static WebApplication UseBlazorWebAssembly(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        // Blazor middleware
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.UseRequestLocalization();
        app.UseCors();

        // Маршрутизация для Blazor
        app.MapRazorPages();
        app.MapFallbackToFile("index.html");

        return app;
    }
}
