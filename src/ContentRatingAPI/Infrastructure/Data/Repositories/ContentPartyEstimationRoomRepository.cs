// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class ContentPartyEstimationRoomRepository : IContentPartyEstimationRoomRepository
    {
        public ContentPartyEstimationRoomRepository(MongoContext mongoContext, IOptions<MongoDBOptions> options)
        {
            this.mongoContext = mongoContext;
            UnitOfWork = mongoContext;
            dbSet = mongoContext.GetCollection<ContentPartyEstimationRoom>(options.Value.ContentPartyEstimationRoomCollectionName);
        }

        public IUnitOfWork UnitOfWork { get; }
        private readonly MongoContext mongoContext;
        private readonly IMongoCollection<ContentPartyEstimationRoom> dbSet;

        public ContentPartyEstimationRoom Add(ContentPartyEstimationRoom room)
        {
            mongoContext.AddCommand((s) => dbSet.InsertOneAsync(s, room), room);
            return room;
        }

        public void Delete(ContentPartyEstimationRoom room)
        {
            mongoContext.AddCommand((s) => dbSet.DeleteOneAsync(s, Builders<ContentPartyEstimationRoom>.Filter.Eq(_ => _.Id, room.Id)), room);
        }

        public async Task<ContentPartyEstimationRoom?> GetRoom(Guid id)
        {
            return await dbSet.Find(Builders<ContentPartyEstimationRoom>.Filter.Eq(_ => _.Id, id)).SingleOrDefaultAsync();
        }

        public ContentPartyEstimationRoom Update(ContentPartyEstimationRoom room)
        {
            mongoContext.AddCommand((s) => dbSet.ReplaceOneAsync(s, Builders<ContentPartyEstimationRoom>.Filter.Eq(_ => _.Id, room.Id), room), room);
            return room;
        }

        public async Task<bool> HasRaterInRoom(Guid roomId, Guid raterId)
        {
            return await dbSet.AsQueryable().AnyAsync(c => c.Id == roomId && c.Raters.Any(c => c.Id == raterId));
        }
    }
}
