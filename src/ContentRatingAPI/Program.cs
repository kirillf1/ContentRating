using System.Net;
using System.Text.Json.Serialization;
using Ardalis.Result.AspNetCore;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications;
using ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation;
using ContentRatingAPI.Application.Identity.RefreshToken;
using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;
using ContentRatingAPI.Application.YoutubeContent;
using ContentRatingAPI.Hubs;
using ContentRatingAPI.Hubs.Filters;
using ContentRatingAPI.Hubs.NotificationServices;
using ContentRatingAPI.Infrastructure.AggregateIntegration;
using ContentRatingAPI.Infrastructure.Authentication;
using ContentRatingAPI.Infrastructure.Authorization;
using ContentRatingAPI.Infrastructure.BlazorConfiguration;
using ContentRatingAPI.Infrastructure.ContentFileManagers;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.MediatrBehaviors;
using ContentRatingAPI.Infrastructure.Telemetry;
using ContentRatingAPI.Infrastructure.YoutubeClient;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Serilog;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .AddUserSecrets(typeof(Program).Assembly)
    .Build();

// add configuration if needed
Log.Logger = LoggingExtensions.CreateSerilogLogger(configuration, environment);

try
{
    Gst.Application.Init(ref args);
}
catch (Exception ex)
{
    Log.Logger.Error(ex, "Failed initialize gstreamer");
}

try
{
    Log.Information("Starting host. Environment: {Env}", environment);
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(Log.Logger);

    // Конфигурация MediatR
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    });

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    // Валидаторы
    builder.Services.AddSingleton<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
    builder.Services.AddSingleton<
        IValidator<CreateContentEstimationListEditorCommand>,
        CreateContentEstimationListEditorCommandValidator
    >();
    builder.Services.AddSingleton<
        IValidator<CreateContentCommand>,
        CreateContentCommandValidator
    >();
    builder.Services.AddSingleton<
        IValidator<UpdateContentCommand>,
        UpdateContentCommandValidator
    >();
    builder.Services.AddSingleton<
        IValidator<StartContentPartyEstimationCommand>,
        StartContentPartyEstimationCommandValidator
    >();

    // Основные сервисы приложения
    builder.AddMongoDbStorage();
    builder.AddApplicationAuthentication();
    builder.AddAggregateIntegrations();
    builder.AddApplicationAuthorization();
    builder.AddTelemetry();

    builder.Services.AddScoped<ContentPartyRatingService>();
    builder.Services.AddHttpClient();
    builder.Services.AddTransient<IYoutubeClient, HttpYoutubeClient>();
    builder.AddContentFileManager();

    // Конфигурация контроллеров
    builder
        .Services.AddControllers(mvcOptions =>
            mvcOptions.AddResultConvention(resultStatusMap =>
                resultStatusMap
                    .AddDefaultMap()
                    .For(
                        ResultStatus.Ok,
                        HttpStatusCode.OK,
                        resultStatusOptions =>
                            resultStatusOptions
                                .For("POST", HttpStatusCode.Created)
                                .For("DELETE", HttpStatusCode.NoContent)
                    )
                    .For(ResultStatus.Error, HttpStatusCode.InternalServerError)
            )
        )
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // Blazor WebAssembly
    builder.AddBlazorWebAssembly();

    // API Documentation (только для разработки)
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // SignalR
    builder.Services.AddSignalR(options => options.AddFilter<LoggingHubFilter>());
    builder.Services.AddTransient<
        IContentPartyEstimationNotificationService,
        ContentPartyEstimationNotificationHubService
    >();
    builder.Services.AddTransient<
        IContentEstimationListEditorNotificationService,
        ContentEstimationListEditorNotificationHubService
    >();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Конфигурация pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "ContentRating API V1");
            options.RoutePrefix = "api/swagger";
        });
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseForwardedHeaders();
    app.UsePathBase("/content-rating");

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseBlazorWebAssembly();

    // Маршрутизация API и SignalR
    app.MapControllers();
    app.MapHub<ContentPartyEstimationHub>("/partyEstimationHub");
    app.MapHub<ContentEstimationListEditorHub>("/contentListEditor");

    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Major Code Smell",
    "S1118:Utility classes should not have public constructors",
    Justification = "<Ожидание>"
)]
public partial class Program { }
