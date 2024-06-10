using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class ContentEditorRoomRepository : IContentEditorRoomRepository
    {

        public ContentEditorRoomRepository(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            dbSet = mongoContext.GetCollection<ContentRoomEditor>(options.Value.ContentEditorRoomCollectionName);
        }
        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IMongoCollection<ContentRoomEditor> dbSet;

        public ContentRoomEditor Add(ContentRoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room), room);
            return room;
        }

        public void Delete(ContentRoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<ContentRoomEditor>.Filter.Eq(_ => _.Id, room.Id)), room);
        }

        public async Task<ContentRoomEditor> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<ContentRoomEditor>.Filter.Eq(_ => _.Id, id)).SingleAsync();
        }

        public ContentRoomEditor Update(ContentRoomEditor room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<ContentRoomEditor>.Filter.Eq(_ => _.Id, room.Id), room), room);
            return room;
        }

        public async Task<bool> HasEditorInRoom(Guid roomId, Guid editorId)
        {
            return await dbSet.AsQueryable().AnyAsync(c => c.Id == roomId && (c.RoomCreator.Id == editorId || c.InvitedEditors.Any(c=> c.Id == editorId)));
        }
    }
}
