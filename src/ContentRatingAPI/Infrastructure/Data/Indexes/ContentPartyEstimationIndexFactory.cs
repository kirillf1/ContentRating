
namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class ContentPartyEstimationIndexFactory : IMongoCollectionIndexFactory
    {
        public Task CreateIndexesInCollection()
        {
            return Task.CompletedTask;
        }
    }
}
