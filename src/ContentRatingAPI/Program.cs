using Ardalis.Result.AspNetCore;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications;
using ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor;
using ContentRatingAPI.Application.ContentPartyEstimationRoom.StartContentPartyEstimation;
using ContentRatingAPI.Application.Identity.RefreshToken;
using ContentRatingAPI.Application.YoutubeContent;
using ContentRatingAPI.Infrastructure.AggregateIntegration;
using ContentRatingAPI.Infrastructure.Authentication;
using ContentRatingAPI.Infrastructure.Authorization;
using ContentRatingAPI.Infrastructure.ContentFileManagers;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.Hubs;
using ContentRatingAPI.Infrastructure.MediatrBehaviors;
using ContentRatingAPI.Infrastructure.Telemetry;
using ContentRatingAPI.Infrastructure.YoutubeClient;
using FluentValidation;
using Serilog;
using System.Net;
using System.Text.Json.Serialization;

// add configuration if needed
Log.Logger = LoggingExtensions.CreateSerilogLogger();

try
{
    Log.Information("Starting host");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(Log.Logger);
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        
    }
    );

    builder.Services.AddSingleton<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
    builder.Services.AddSingleton<IValidator<CreateContentEstimationListEditorCommand>, CreateContentEstimationListEditorCommandValidator>();
    builder.Services.AddSingleton<IValidator<CreateContentCommand>,  CreateContentCommandValidator>();
    builder.Services.AddSingleton<IValidator<UpdateContentCommand>,  UpdateContentCommandValidator>();
    builder.Services.AddSingleton<IValidator<StartContentPartyEstimationCommand>, StartContentPartyEstimationCommandValidator>();

    builder.AddApplicationAuthentication();
    builder.AddMongoDbStorage();
    builder.AddAggregateIntegrations();
    builder.AddApplicationAuthorization();

    // if more services add new extension
    builder.Services.AddScoped<ContentPartyRatingService>();

    builder.Services.AddHttpClient();
    builder.Services.AddTransient<IYoutubeClient, HttpYoutubeClient>();
    builder.AddContentFileManager();

    builder.Services.AddControllers(mvcOptions => mvcOptions
    .AddResultConvention(resultStatusMap => resultStatusMap
        .AddDefaultMap()
        .For(ResultStatus.Ok, HttpStatusCode.OK, resultStatusOptions => resultStatusOptions
            .For("POST", HttpStatusCode.Created)
            .For("DELETE", HttpStatusCode.NoContent))
        .For(ResultStatus.Error, HttpStatusCode.InternalServerError)
    )).AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { "en-US", "ru-RU" };
        options.SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
        options.ApplyCurrentCultureToResponseHeaders = true;

    });

    builder.Services.AddSignalR();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => 
        { 
        });
    }
    app.UseRequestLocalization();
    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHub<ContentPartyEstimationHub>("/partyEstimationHub");

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
