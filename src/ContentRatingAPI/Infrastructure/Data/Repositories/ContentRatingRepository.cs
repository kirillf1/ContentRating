using ContentRating.Domain.AggregatesModel.ContentRatingAggregate;
using ContentRating.Domain.Shared;
using MongoDB.Driver;
using ContentRatingAggregate = ContentRating.Domain.AggregatesModel.ContentRatingAggregate.ContentRating;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class ContentRatingRepository : IContentRatingRepository
    {
        private const string COLLECTION_NAME = "content_ratings";
        public ContentRatingRepository(MongoContext mongoContext, IChangeTracker changeTracker)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            this.changeTracker = changeTracker;
            dbSet = mongoContext.GetCollection<ContentRatingAggregate>(COLLECTION_NAME);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IChangeTracker changeTracker;
        private readonly IMongoCollection<ContentRatingAggregate> dbSet;
        public ContentRatingAggregate Add(ContentRatingAggregate contentRating)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, contentRating));
            changeTracker.TrackEntity(contentRating);
            return contentRating;
        }

        public void Delete(ContentRatingAggregate contentRating)
        {   
            changeTracker.TrackEntity(contentRating);
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, 
                Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, contentRating.Id)));
        }

        public async Task<ContentRatingAggregate> GetContentRating(Guid id)
        {
            return await dbSet.Find(Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public ContentRatingAggregate Update(ContentRatingAggregate contentRating)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, contentRating.Id), contentRating));
            changeTracker.TrackEntity(contentRating);
            return contentRating;
        }

        public async Task<IEnumerable<ContentRatingAggregate>> GetContentRatingsByRoom(Guid roomId)
        {
            var ratings = await dbSet.Find(Builders<ContentRatingAggregate>.Filter.Eq(_ => _.Id, roomId)).ToListAsync();
            foreach (var rating in ratings)
            {
                changeTracker.TrackEntity(rating);
            }
            return ratings;
        }
    }
}
