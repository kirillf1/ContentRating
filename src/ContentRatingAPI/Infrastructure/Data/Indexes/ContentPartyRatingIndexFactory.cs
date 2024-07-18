using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class ContentPartyRatingIndexFactory : IMongoCollectionIndexFactory
    {
        private readonly IMongoClient mongoClient;
        private readonly IOptions<MongoDBOptions> options;
        private readonly ILogger<ContentPartyRatingIndexFactory> logger;

        public ContentPartyRatingIndexFactory(IMongoClient mongoClient, IOptions<MongoDBOptions> options,
            ILogger<ContentPartyRatingIndexFactory> logger)
        {
            this.mongoClient = mongoClient;
            this.options = options;
            this.logger = logger;
        }
        public async Task CreateIndexesInCollection()
        {
            var database = mongoClient.GetDatabase(options.Value.DatabaseName);
            var collection = database.GetCollection<ContentPartyRating>(options.Value.ContentPartyRatingCollectionName);
            var contentPartyRatingIndexDefinition = Builders<ContentPartyRating>.IndexKeys;
            var contentPartyRatingOptions = new CreateIndexOptions { Unique = false };
            
            var indexModel = new CreateIndexModel<ContentPartyRating>(contentPartyRatingIndexDefinition.Descending(c => c.RoomId), contentPartyRatingOptions);
            await collection.Indexes.CreateOneAsync(indexModel);
            logger.LogInformation("Index for {property} created", nameof(ContentPartyRating.RoomId));
           
            indexModel = new CreateIndexModel<ContentPartyRating>(contentPartyRatingIndexDefinition.Descending(c => c.ContentId), contentPartyRatingOptions);
            await collection.Indexes.CreateOneAsync(indexModel);
            logger.LogInformation("Index for {property} created", nameof(ContentPartyRating.ContentId));
        }
    }
}
