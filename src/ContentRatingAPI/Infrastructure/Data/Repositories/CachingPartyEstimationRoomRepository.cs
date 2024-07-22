using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class CachingPartyEstimationRoomRepository : IContentPartyEstimationRoomRepository
    {
        public readonly static string ContentPartyEstimationKey = nameof(ContentPartyEstimationRoom);
        private readonly IContentPartyEstimationRoomRepository baseRepository;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions cacheEntryOptions;
        public CachingPartyEstimationRoomRepository(IContentPartyEstimationRoomRepository baseRepository, IMemoryCache memoryCache)
        {
            this.baseRepository = baseRepository;
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
                .SetSize(2000)
                .SetSlidingExpiration(TimeSpan.FromSeconds(60));
        }
        public IUnitOfWork UnitOfWork => baseRepository.UnitOfWork;

        public ContentPartyEstimationRoom Add(ContentPartyEstimationRoom room)
        {
            memoryCache.Set(GetKeyById(room.Id), room, cacheEntryOptions);
            return baseRepository.Add(room);
        }

        public void Delete(ContentPartyEstimationRoom room)
        {
            memoryCache.Remove(GetKeyById(room.Id));
            baseRepository.Delete(room);
        }

        public async Task<ContentPartyEstimationRoom?> GetRoom(Guid id)
        {
            if (memoryCache.TryGetValue(GetKeyById(id), out ContentPartyEstimationRoom? room) && room is not null)
                return room;

            room = await baseRepository.GetRoom(id);

            if (room is not null)
                memoryCache.Set(GetKeyById(id), room, cacheEntryOptions);

            return room;
        }

        public async Task<bool> HasRaterInRoom(Guid roomId, Guid raterId)
        {
            if (memoryCache.TryGetValue(GetKeyById(roomId), out ContentPartyEstimationRoom? partyEstimationRoom) && partyEstimationRoom is not null)
            {
                return partyEstimationRoom!.RoomCreator.Id == raterId || partyEstimationRoom.Raters.Any(c => c.Id == raterId);
            }
            return await baseRepository.HasRaterInRoom(roomId, raterId);
        }

        public ContentPartyEstimationRoom Update(ContentPartyEstimationRoom room)
        {
            memoryCache.Set(GetKeyById(room.Id), room, cacheEntryOptions);
            return baseRepository.Update(room);
        }
        private static string GetKeyById(Guid id)
        {
            return ContentPartyEstimationKey + id.ToString();
        }
    }
}
