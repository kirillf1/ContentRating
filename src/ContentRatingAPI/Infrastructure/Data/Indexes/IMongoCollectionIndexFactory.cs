namespace ContentRatingAPI.Infrastructure.Data.Indexes
{
    public interface IMongoCollectionIndexFactory
    {
        public Task CreateIndexesInCollection();
    }
}
