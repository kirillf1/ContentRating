using ContentRating.Web.UI.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace ContentRating.Web.UI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var apiSettings = new ApiSettings();
        builder.Configuration.GetSection("ApiSettings").Bind(apiSettings);
        builder.Services.AddSingleton(apiSettings);

        // Базовый HttpClient без аутентификации
        builder.Services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<ApiSettings>();
            return new HttpClient { BaseAddress = new Uri(settings.BaseUrl) };
        });

        builder.Services.AddMudServices();

        builder.Services.AddScoped<ContentRating.Web.UI.Services.ThemeService>();
        builder.Services.AddScoped<ContentRating.Web.UI.Services.SecureTokenStorage>();
        builder.Services.AddScoped<ContentRating.Web.UI.Services.AuthService>();

        var app = builder.Build();

        await app.RunAsync();
    }
}
