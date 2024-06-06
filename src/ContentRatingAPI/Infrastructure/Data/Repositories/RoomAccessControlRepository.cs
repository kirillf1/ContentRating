using ContentRating.Domain.AggregatesModel.ContentPartyRatingRoomAggregate;
using ContentRating.Domain.Shared;
using MongoDB.Driver;
using System.Collections.Generic;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class RoomAccessControlRepository : IRoomAccessControlRepository
    {
        private const string COLLECTION_NAME = "access_control_rooms";
        public RoomAccessControlRepository(MongoContext mongoContext, IChangeTracker changeTracker)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            this.changeTracker = changeTracker;
            dbSet = mongoContext.GetCollection<ContentPartyRatingRoom>(COLLECTION_NAME);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IChangeTracker changeTracker;
        private readonly IMongoCollection<ContentPartyRatingRoom> dbSet;

        public ContentPartyRatingRoom Add(ContentPartyRatingRoom room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room));
            changeTracker.TrackEntity(room);
            return room;
        }

        public void Delete(ContentPartyRatingRoom room)
        {
            changeTracker.TrackEntity(room);
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<ContentPartyRatingRoom>.Filter.Eq(_ => _.Id, room.Id)));
        }

        public async Task<ContentPartyRatingRoom> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<ContentPartyRatingRoom>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public ContentPartyRatingRoom Update(ContentPartyRatingRoom room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<ContentPartyRatingRoom>.Filter.Eq(_ => _.Id, room.Id), room));
            changeTracker.TrackEntity(room);
            return room;
        }
    }
}
