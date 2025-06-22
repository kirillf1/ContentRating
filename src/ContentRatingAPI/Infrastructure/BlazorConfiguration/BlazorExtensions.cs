using ContentRating.Web.UI;

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
        builder.Services.AddCors(options => { });

        return builder;
    }

    public static WebApplication UseBlazorWebAssembly(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        app.UseAntiforgery();
        // Blazor middleware
        app.MapStaticAssets();
        app.UseRequestLocalization();
        app.UseCors();

        app.MapRazorPages();
        app.MapFallbackToFile("index.html");
        // Маршрутизация для Blazor
        //app.MapRazorComponents<App>()
        //    .AddInteractiveServerRenderMode()
        //    .AddInteractiveWebAssemblyRenderMode();

        return app;
    }
}
