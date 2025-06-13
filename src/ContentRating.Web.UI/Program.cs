using ContentRating.Web.UI.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
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

        // HttpClient с аутентификацией для API сервисов
        builder.Services.AddScoped<ContentRating.Web.UI.Services.AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<
                Services.IContentEstimationListService,
                Services.ContentEstimationListService
            >(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<
                Services.IYoutubeService,
                Services.YoutubeService
            >(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<
                Services.IContentFileService,
                Services.ContentFileService
            >(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(300); // Увеличенный таймаут для загрузки файлов
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 4000; // 4 секунды
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        builder.Services.AddScoped<ContentRating.Web.UI.Services.ThemeService>();
        builder.Services.AddScoped<ContentRating.Web.UI.Services.SecureTokenStorage>();
        builder.Services.AddScoped<ContentRating.Web.UI.Services.AuthService>();

        // SignalR сервис для редактора контента
        builder.Services.AddScoped<
            ContentRating.Web.UI.Services.IContentEstimationListEditorHubService,
            ContentRating.Web.UI.Services.ContentEstimationListEditorHubService
        >();

        // ViewModels
        builder.Services.AddScoped<ContentRating.Web.UI.ViewModels.ContentEstimationListEditorViewModel>();
        builder.Services.AddScoped<ContentRating.Web.UI.ViewModels.YoutubeImportViewModel>();
        builder.Services.AddScoped<ContentRating.Web.UI.ViewModels.FileUploadViewModel>();
        
        // Services
        builder.Services.AddScoped<ContentRating.Web.UI.Services.ContentValidationService>();
        builder.Services.AddScoped<ContentRating.Web.UI.Services.ContentItemEditingService>();

        var app = builder.Build();

        await app.RunAsync();
    }
}
