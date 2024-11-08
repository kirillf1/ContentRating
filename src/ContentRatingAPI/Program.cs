// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using ContentRatingAPI.Infrastructure.ContentFileManagers;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.MediatrBehaviors;
using ContentRatingAPI.Infrastructure.Telemetry;
using ContentRatingAPI.Infrastructure.YoutubeClient;
using FluentValidation;
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
    Log.Information("Starting host. Environment: {Env}", environment);
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(Log.Logger);
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    });

    builder.Services.AddSingleton<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
    builder.Services.AddSingleton<IValidator<CreateContentEstimationListEditorCommand>, CreateContentEstimationListEditorCommandValidator>();
    builder.Services.AddSingleton<IValidator<CreateContentCommand>, CreateContentCommandValidator>();
    builder.Services.AddSingleton<IValidator<UpdateContentCommand>, UpdateContentCommandValidator>();
    builder.Services.AddSingleton<IValidator<StartContentPartyEstimationCommand>, StartContentPartyEstimationCommandValidator>();

    builder.AddApplicationAuthentication();
    builder.AddMongoDbStorage();
    builder.AddAggregateIntegrations();
    builder.AddApplicationAuthorization();
    builder.AddTelemetry();

    // if more services add new extension
    builder.Services.AddScoped<ContentPartyRatingService>();

    builder.Services.AddHttpClient();
    builder.Services.AddTransient<IYoutubeClient, HttpYoutubeClient>();
    builder.AddContentFileManager();

    builder
        .Services.AddControllers(mvcOptions =>
            mvcOptions.AddResultConvention(resultStatusMap =>
                resultStatusMap
                    .AddDefaultMap()
                    .For(
                        ResultStatus.Ok,
                        HttpStatusCode.OK,
                        resultStatusOptions => resultStatusOptions.For("POST", HttpStatusCode.Created).For("DELETE", HttpStatusCode.NoContent)
                    )
                    .For(ResultStatus.Error, HttpStatusCode.InternalServerError)
            )
        )
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "en-US", "ru-RU" };
        options.SetDefaultCulture(supportedCultures[0]).AddSupportedCultures(supportedCultures).AddSupportedUICultures(supportedCultures);
        options.ApplyCurrentCultureToResponseHeaders = true;
    });

    builder.Services.AddSignalR(options => options.AddFilter<LoggingHubFilter>());
    builder.Services.AddTransient<IContentPartyEstimationNotificationService, ContentPartyEstimationNotificationHubService>();
    builder.Services.AddTransient<IContentEstimationListEditorNotificationService, ContentEstimationListEditorNotificationHubService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => { });
    }
    app.UseRequestLocalization();
    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<ContentPartyEstimationHub>("/partyEstimationHub");

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
