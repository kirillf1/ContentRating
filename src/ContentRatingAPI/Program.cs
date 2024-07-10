using Ardalis.Result.AspNetCore;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Infrastructure.AggregateIntegration;
using ContentRatingAPI.Infrastructure.Authentication;
using ContentRatingAPI.Infrastructure.Authorization;
using ContentRatingAPI.Infrastructure.ContentFileManagers;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.MediatrBehaviors;
using ContentRatingAPI.Infrastructure.Telemetry;
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
        cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    }
    );

    builder.AddApplicationAuthentication();
    builder.AddMongoDbStorage();
    builder.AddAggregateIntegrations();
    builder.AddApplicationAuthorization();

    // if more services add new extension
    builder.Services.AddScoped<ContentPartyRatingService>();
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

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseSerilogRequestLogging();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();


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
