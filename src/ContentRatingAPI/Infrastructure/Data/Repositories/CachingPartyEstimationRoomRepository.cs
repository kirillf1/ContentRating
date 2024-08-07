using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.Shared;
using ContentRatingAPI.Infrastructure.Data.Caching;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class CachingPartyEstimationRoomRepository : IContentPartyEstimationRoomRepository
    {
        private readonly IContentPartyEstimationRoomRepository baseRepository;
        private readonly GenericCacheBase<ContentPartyEstimationRoom> cache;

        public CachingPartyEstimationRoomRepository(IContentPartyEstimationRoomRepository baseRepository, GenericCacheBase<ContentPartyEstimationRoom> genericCache)
        {
            this.baseRepository = baseRepository;
            cache = genericCache;
          
        }
        public IUnitOfWork UnitOfWork => baseRepository.UnitOfWork;

        public ContentPartyEstimationRoom Add(ContentPartyEstimationRoom room)
        {
            cache.Set(room.Id, room);
            return baseRepository.Add(room);
        }

        public void Delete(ContentPartyEstimationRoom room)
        {
            cache.Remove(room.Id);
            baseRepository.Delete(room);
        }

        public async Task<ContentPartyEstimationRoom?> GetRoom(Guid id)
        {
            if (cache.TryGetValue(id, out ContentPartyEstimationRoom? room) && room is not null)
                return room;

            room = await baseRepository.GetRoom(id);

            if (room is not null)
                cache.Set(id, room);

            return room;
        }

        public async Task<bool> HasRaterInRoom(Guid roomId, Guid raterId)
        {
            if (cache.TryGetValue(roomId, out ContentPartyEstimationRoom? partyEstimationRoom) && partyEstimationRoom is not null)
            {
                return partyEstimationRoom!.RoomCreator.Id == raterId || partyEstimationRoom.Raters.Any(c => c.Id == raterId);
            }
            return await baseRepository.HasRaterInRoom(roomId, raterId);
        }

        public ContentPartyEstimationRoom Update(ContentPartyEstimationRoom room)
        {
            cache.Set(room.Id, room);
            return baseRepository.Update(room);
        }
       
    }
}
