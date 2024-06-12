using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ContentRatingAggregate = ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.ContentPartyRating;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class ContentPartyRatingRepository : IContentPartyRatingRepository
    {
        public ContentPartyRatingRepository(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            dbSet = mongoContext.GetCollection<ContentRatingAggregate>(options.Value.ContentPartyRatingCollectionName);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IMongoCollection<ContentRatingAggregate> dbSet;
        public ContentRatingAggregate Add(ContentRatingAggregate contentRating)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, contentRating), contentRating);

            return contentRating;
        }

        public void Delete(ContentRatingAggregate contentRating)
        {   
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, 
                Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, contentRating.Id)), contentRating);
        }

        public async Task<ContentRatingAggregate> GetContentRating(Guid id)
        {
            return await dbSet.Find(Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public ContentRatingAggregate Update(ContentRatingAggregate contentRating)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, contentRating.Id), contentRating), contentRating);
            return contentRating;
        }

        public async Task<IEnumerable<ContentRatingAggregate>> GetContentRatingsByRoom(Guid roomId)
        {
            var ratings = await dbSet.Find(Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, roomId)).ToListAsync();         
            return ratings;
        }

        public async Task<ContentRatingAggregate> GetContentRating(Guid roomId, Guid contentId)
        {
            return await dbSet.Find(Builders<ContentRatingAggregate>.Filter.And(
                    Builders<ContentRatingAggregate>.Filter.Eq(_ => _.RoomId, roomId),
                    Builders<ContentRatingAggregate>.Filter.Eq(_ => _.ContentId, contentId)))
                .SingleAsync();
        }
    }
}
