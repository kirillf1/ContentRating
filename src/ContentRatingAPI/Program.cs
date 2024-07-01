using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Infrastructure.AggregateIntegration;
using ContentRatingAPI.Infrastructure.Authentication;
using ContentRatingAPI.Infrastructure.Authorization;
using ContentRatingAPI.Infrastructure.ContentFileManagers;
using ContentRatingAPI.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.AddApplicationAuthentication();
builder.AddMongoDbStorage();
builder.AddAggregateIntegrations();
builder.AddApplicationAuthorization();

// if more services add new extension
builder.Services.AddScoped<ContentPartyRatingService>();
builder.AddContentFileManager();

builder.Services.AddControllers();
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
