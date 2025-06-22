using ContentRating.Web.UI.Models;
using ContentRating.Web.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace ContentRating.Web.UI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        var apiBaseUrl = builder.HostEnvironment.BaseAddress;

        // Создаем HttpClient для получения конфигурации
        var configHttpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
        var configService = new ConfigurationService(configHttpClient);

        // Получаем конфигурацию из API
        var apiSettings = await configService.GetConfigurationAsync();
        builder.Services.AddSingleton(apiSettings);

        // Базовый HttpClient без аутентификации
        builder.Services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<ApiSettings>();
            return new HttpClient { BaseAddress = new Uri(settings.BaseUrl) };
        });

        // HttpClient с аутентификацией для API сервисов
        builder.Services.AddScoped<Services.AuthenticatedHttpClientHandler>();

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
            .Services.AddHttpClient<Services.IYoutubeService, Services.YoutubeService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<Services.IContentFileService, Services.ContentFileService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(300); // Увеличенный таймаут для загрузки файлов
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<
                Services.IContentPartyEstimationService,
                Services.ContentPartyEstimationService
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
                Services.IContentPartyRatingService,
                Services.ContentPartyRatingService
            >(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        // UserService для работы с пользователями
        builder
            .Services.AddHttpClient<Services.IUserService, Services.UserService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
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

        // HttpClient для AuthService
        builder.Services.AddHttpClient("AuthHttpClient", (sp, client) =>
        {
            var settings = sp.GetRequiredService<ApiSettings>();
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddScoped<Services.ThemeService>();
        builder.Services.AddSingleton<Services.SecureTokenStorage>();
        builder.Services.AddSingleton<Services.AuthService>();

        // SignalR сервис для редактора контента
        builder.Services.AddTransient<
            Services.IContentEstimationListEditorHubService,
            Services.ContentEstimationListEditorHubService
        >();

        // SignalR сервис для оценки контента
        builder.Services.AddTransient<
            Services.IContentPartyEstimationHubService,
            Services.ContentPartyEstimationHubService
        >();

        // ViewModels
        builder.Services.AddTransient<ViewModels.ContentEstimationListEditorViewModel>();
        builder.Services.AddTransient<ViewModels.YoutubeImportViewModel>();
        builder.Services.AddTransient<ViewModels.FileUploadViewModel>();
        builder.Services.AddTransient<ViewModels.ContentPartyEstimationRoomViewModel>();

        // Services
        builder.Services.AddTransient<Services.ContentItemEditingService>();

        // ContentValidationService с аутентифицированным HttpClient
        builder
            .Services.AddHttpClient<Services.ContentValidationService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<Services.AuthenticatedHttpClientHandler>();

        builder.Services.AddPWAUpdater();
        var app = builder.Build();

        await app.RunAsync();
    }
}
