using ContentRatingAPI.Application.ContentPartyEstimationRoom.ContentService;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;
using ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyEstimationRoom;
using ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration
{
    public static class AggregateIntegrationExtensions
    {
        public static IServiceCollection AddAggregateIntegrations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IContentForEstimationService, TranslatingContentForEstimationService>();
            builder.Services.AddSingleton<ContentRaterTranslator>();
            builder.Services.AddScoped<ContentRaterAdapter>();
            builder.Services.AddScoped<IContentRaterService, TranslatingContentRaterService>();
            return builder.Services;
        }
    }
}
