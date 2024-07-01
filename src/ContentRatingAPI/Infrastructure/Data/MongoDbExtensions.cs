using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.Shared;
using ContentRatingAPI.Infrastructure.Data.MapConvensions;
using ContentRatingAPI.Infrastructure.Data.Repositories;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using ContentRatingAPI.Application.ContentFileManager;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace ContentRatingAPI.Infrastructure.Data
{
    public static class MongoDbExtensions
    {
        public static IServiceCollection AddMongoDbStorage(this IHostApplicationBuilder builder)
        {
            RegisterEntityClassMap();
            builder.Services.AddSingleton<IMongoClient>(c =>
            {
                var options = c.GetRequiredService<IOptions<MongoDBOptions>>();
                return new MongoClient(options.Value.Connection);
            });
            builder.Services.Configure<MongoDBOptions>(builder.Configuration.GetSection(MongoDBOptions.Position));
            builder.Services.AddScoped<IUnitOfWork, MongoContext>();
            builder.Services.AddScoped<MongoContext>();
            builder.Services.AddScoped<IChangeTracker, InMemoryChangeTracker>();
            

            builder.Services.AddTransient<IContentEditorRoomRepository, ContentEditorRoomRepository>();
            builder.Services.AddTransient<IContentPartyEstimationRoomRepository, ContentPartyEstimationRoomRepository>();
            builder.Services.AddTransient<IContentPartyRatingRepository, ContentPartyRatingRepository>();
            return builder.Services;
        }
        private static void RegisterEntityClassMap()
        {
            var conventionPack = new ConventionPack
            {
                new MapReadOnlyPropertiesConvention()
            };
            ConventionRegistry.Register("Conventions", conventionPack, _ => true);

            BsonClassMap.RegisterClassMap<ContentPartyEstimationRoom>(classMap =>
            {
                classMap.AutoMap();
            });

            BsonClassMap.RegisterClassMap<ContentPartyRating>(classMap =>
            {
                classMap.AutoMap();
                classMap.SetDictionaryRepresentation(c => c.RaterScores, DictionaryRepresentation.ArrayOfDocuments);

            });

            BsonClassMap.RegisterClassMap<ContentRoomEditor>(classMap =>
            {
                classMap.AutoMap();
            });
            
            BsonClassMap.RegisterClassMap<SavedContentFileInfo>(classMap =>
            {
                classMap.AutoMap();
            });
        }
    }
}
