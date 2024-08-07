
namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public class MongoIndexService : IHostedService
    {
        private readonly IEnumerable<IMongoCollectionIndexFactory> indexFactories;

        public MongoIndexService(IEnumerable<IMongoCollectionIndexFactory> indexFactories)
        {
            this.indexFactories = indexFactories;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var indexFactory in indexFactories)
            {
                await indexFactory.CreateIndexesInCollection();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
