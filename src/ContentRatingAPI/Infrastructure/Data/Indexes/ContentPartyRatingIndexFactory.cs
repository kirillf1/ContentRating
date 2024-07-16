using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class ContentPartyRatingIndexFactory : IMongoCollectionIndexFactory
    {
        private readonly MongoContext mongoContext;
        private readonly IOptions<MongoDBOptions> options;
        private readonly ILogger<ContentPartyRatingIndexFactory> logger;

        public ContentPartyRatingIndexFactory(MongoContext mongoContext, IOptions<MongoDBOptions> options,
            ILogger<ContentPartyRatingIndexFactory> logger)
        {
            this.mongoContext = mongoContext;
            this.options = options;
            this.logger = logger;
        }
        public async Task CreateIndexesInCollection()
        {
            var collection = mongoContext.GetCollection<ContentPartyRating>(options.Value.ContentPartyRatingCollectionName);

            var roomIdIndexDefinition = Builders<ContentPartyRating>.IndexKeys.Descending(x => x.RoomId);
            var roomIdIndexOptions = new CreateIndexOptions { Unique = false };
            await collection.Indexes.CreateOneAsync(roomIdIndexDefinition, roomIdIndexOptions);
        }
    }
}
