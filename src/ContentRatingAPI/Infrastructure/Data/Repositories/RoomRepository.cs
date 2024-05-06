using ContentRating.Domain.AggregatesModel.RoomEditorAggregate;
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
            dbSet = mongoContext.GetCollection<RoomEditor>(COLLECTION_NAME);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IChangeTracker changeTracker;
        private readonly IMongoCollection<RoomEditor> dbSet;

        public RoomEditor Add(RoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room));
            changeTracker.TrackEntity(room);
            return room;
        }

        public void Delete(RoomEditor room)
        {
            changeTracker.TrackEntity(room);
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<RoomEditor>.Filter.Eq(_ => _.Id, room.Id)));
        }

        public async Task<RoomEditor> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<RoomEditor>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public RoomEditor Update(RoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<RoomEditor>.Filter.Eq(_ => _.Id, room.Id), room));
            changeTracker.TrackEntity(room);
            return room;
        }
    }
}
