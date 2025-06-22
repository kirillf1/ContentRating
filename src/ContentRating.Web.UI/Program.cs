using ContentRating.Web.UI.Models;
using ContentRating.Web.UI.Services.Authentication;
using ContentRating.Web.UI.Services.Communication;
using ContentRating.Web.UI.Services.Configuration;
using ContentRating.Web.UI.Services.Content;
using ContentRating.Web.UI.Services.Storage;
using ContentRating.Web.UI.Services.Theme;
using ContentRating.Web.UI.Services.Validation;
using ContentRating.Web.UI.ViewModels.Estimation;
using ContentRating.Web.UI.ViewModels.Import;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
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

        // Добавляем логирование
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        var apiBaseUrl = builder.HostEnvironment.BaseAddress;

        // Создаем HttpClient для получения конфигурации
        var configHttpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
        var loggerFactory = LoggerFactory.Create(_ => { });
        var logger = loggerFactory.CreateLogger<ConfigurationService>();
        var configService = new ConfigurationService(configHttpClient, logger);

        // Получаем конфигурацию из API
        var apiSettings = await configService.GetApiSettingsAsync();

        // Если конфигурация не получена, используем значения по умолчанию
        if (apiSettings == null)
        {
            apiSettings = new ApiSettings { BaseUrl = apiBaseUrl, SignalRHubUrl = apiBaseUrl };
        }

        builder.Services.AddSingleton(apiSettings);

        // Базовый HttpClient без аутентификации
        builder.Services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<ApiSettings>();
            return new HttpClient { BaseAddress = new Uri(settings.BaseUrl) };
        });

        // HttpClient с аутентификацией для API сервисов
        builder.Services.AddScoped<AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<IContentEstimationListService, ContentEstimationListService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<IYoutubeService, YoutubeService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<IContentFileService, ContentFileService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(300); // Увеличенный таймаут для загрузки файлов
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<IContentPartyEstimationService, ContentPartyEstimationService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

        builder
            .Services.AddHttpClient<IContentPartyRatingService, ContentPartyRatingService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

        // UserService для работы с пользователями
        builder
            .Services.AddHttpClient<IUserService, UserService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

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
        builder.Services.AddHttpClient(
            "AuthHttpClient",
            (sp, client) =>
            {
                var settings = sp.GetRequiredService<ApiSettings>();
                client.BaseAddress = new Uri(settings.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            }
        );

        builder.Services.AddScoped<ThemeService>();
        builder.Services.AddSingleton<SecureTokenStorage>();
        builder.Services.AddSingleton<AuthService>();

        // SignalR сервис для редактора контента
        builder.Services.AddTransient<
            IContentEstimationListEditorHubService,
            ContentEstimationListEditorHubService
        >();

        // SignalR сервис для оценки контента
        builder.Services.AddTransient<
            IContentPartyEstimationHubService,
            ContentPartyEstimationHubService
        >();

        // ViewModels
        builder.Services.AddTransient<ContentEstimationListEditorViewModel>();
        builder.Services.AddTransient<YoutubeImportViewModel>();
        builder.Services.AddTransient<FileUploadViewModel>();
        builder.Services.AddTransient<ContentPartyEstimationRoomViewModel>();

        // Services
        builder.Services.AddTransient<ContentItemEditingService>();

        // ContentValidationService с аутентифицированным HttpClient
        builder
            .Services.AddHttpClient<ContentValidationService>(
                (sp, client) =>
                {
                    var settings = sp.GetRequiredService<ApiSettings>();
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            )
            .ConfigurePrimaryHttpMessageHandler<AuthenticatedHttpClientHandler>();

        builder.Services.AddPWAUpdater();
        var app = builder.Build();

        await app.RunAsync();
    }
}
