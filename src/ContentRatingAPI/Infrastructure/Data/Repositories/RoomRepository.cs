using ContentRating.Domain.AggregatesModel.ContentRoomAggregate;
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
            dbSet = mongoContext.GetCollection<Room>(COLLECTION_NAME);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IChangeTracker changeTracker;
        private readonly IMongoCollection<Room> dbSet;

        public Room Add(Room room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room));
            changeTracker.TrackEntity(room);
            return room;
        }

        public void Delete(Room room)
        {
            changeTracker.TrackEntity(room);
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<Room>.Filter.Eq(_ => _.Id, room.Id)));
        }

        public async Task<Room> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<Room>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public Room Update(Room room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<Room>.Filter.Eq(_ => _.Id, room.Id), room));
            changeTracker.TrackEntity(room);
            return room;
        }
    }
}
