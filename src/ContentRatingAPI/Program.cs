using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Infrastructure.AggregateIntegration;
using ContentRatingAPI.Infrastructure.Authentication;
using ContentRatingAPI.Infrastructure.Authorization;
using ContentRatingAPI.Infrastructure.ContentFileManagers;
using ContentRatingAPI.Infrastructure.Data;
using ContentRatingAPI.Infrastructure.MediatrBehaviors;
using MediatR.Pipeline;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalRequestExceptionHandler<,,>));
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
