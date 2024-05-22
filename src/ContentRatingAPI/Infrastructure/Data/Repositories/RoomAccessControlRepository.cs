using ContentRating.Domain.AggregatesModel.RoomAccessControlAggregate;
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
            dbSet = mongoContext.GetCollection<RoomAccessControl>(COLLECTION_NAME);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IChangeTracker changeTracker;
        private readonly IMongoCollection<RoomAccessControl> dbSet;

        public RoomAccessControl Add(RoomAccessControl room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room));
            changeTracker.TrackEntity(room);
            return room;
        }

        public void Delete(RoomAccessControl room)
        {
            changeTracker.TrackEntity(room);
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<RoomAccessControl>.Filter.Eq(_ => _.Id, room.Id)));
        }

        public async Task<RoomAccessControl> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<RoomAccessControl>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public RoomAccessControl Update(RoomAccessControl room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<RoomAccessControl>.Filter.Eq(_ => _.Id, room.Id), room));
            changeTracker.TrackEntity(room);
            return room;
        }
    }
}
