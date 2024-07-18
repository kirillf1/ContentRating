
using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class ContentPartyEstimationIndexFactory : IMongoCollectionIndexFactory
    {
        private readonly IMongoClient mongoClient;
        private readonly IOptions<MongoDBOptions> options;
        private readonly ILogger<ContentPartyRatingIndexFactory> logger;

        public ContentPartyEstimationIndexFactory(IMongoClient mongoClient, IOptions<MongoDBOptions> options,
            ILogger<ContentPartyRatingIndexFactory> logger)
        {
            this.mongoClient = mongoClient;
            this.options = options;
            this.logger = logger;
        }
        public async Task CreateIndexesInCollection()
        {
            var database = mongoClient.GetDatabase(options.Value.DatabaseName);
            var collection = database.GetCollection<ContentPartyEstimationRoom>(options.Value.ContentPartyEstimationRoomCollectionName);
            var contentPartyRatingIndexDefinition = Builders<ContentPartyEstimationRoom>.IndexKeys;
            var contentPartyRatingOptions = new CreateIndexOptions { Unique = false };

            var indexModel = new CreateIndexModel<ContentPartyEstimationRoom>(contentPartyRatingIndexDefinition.Ascending($"{nameof(ContentPartyEstimationRoom.ContentForEstimation)}.{nameof(ContentForEstimation.Url)}"), contentPartyRatingOptions);
            await collection.Indexes.CreateOneAsync(indexModel);
            logger.LogInformation("Index for {property} created", nameof(ContentForEstimation.Url));
        }
    }
}
