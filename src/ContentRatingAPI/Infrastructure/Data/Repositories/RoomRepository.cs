using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.Shared;
using MongoDB.Driver;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private const string COLLECTION_NAME = "rooms";
        public RoomRepository(MongoContext mongoContext, IChangeTracker changeTracker)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            this.changeTracker = changeTracker;
            dbSet = mongoContext.GetCollection<ContentRoomEditor>(COLLECTION_NAME);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IChangeTracker changeTracker;
        private readonly IMongoCollection<ContentRoomEditor> dbSet;

        public ContentRoomEditor Add(ContentRoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room));
            changeTracker.TrackEntity(room);
            return room;
        }

        public void Delete(ContentRoomEditor room)
        {
            changeTracker.TrackEntity(room);
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<ContentRoomEditor>.Filter.Eq(_ => _.Id, room.Id)));
        }

        public async Task<ContentRoomEditor> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<ContentRoomEditor>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public ContentRoomEditor Update(ContentRoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<ContentRoomEditor>.Filter.Eq(_ => _.Id, room.Id), room));
            changeTracker.TrackEntity(room);
            return room;
        }
    }
}
