using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class CachingContentPartyRatingRepository : IContentPartyRatingRepository
    {
        private readonly IContentPartyRatingRepository baseRepository;
        public readonly static string ContentPartyRatingKey = nameof(ContentPartyRating);
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions cacheEntryOptions;
        public CachingContentPartyRatingRepository(IContentPartyRatingRepository baseRepository, IMemoryCache memoryCache)
        {
            this.baseRepository = baseRepository;
            this.memoryCache = memoryCache;
            cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                .SetSize(3000)
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));
        }
        public IUnitOfWork UnitOfWork => baseRepository.UnitOfWork;

        public ContentPartyRating Add(ContentPartyRating contentRating)
        {
            memoryCache.Set(GetKeyById(contentRating.Id), contentRating, cacheEntryOptions);
            return baseRepository.Add(contentRating);
        }

        public void Delete(ContentPartyRating contentRating)
        {
            memoryCache.Remove(GetKeyById(contentRating.Id));
            baseRepository.Delete(contentRating);
        }

        public async Task<ContentPartyRating?> GetContentRating(Guid id)
        {
            return await memoryCache.GetOrCreateAsync(GetKeyById(id), async entry =>
            {
                entry.SetOptions(cacheEntryOptions);
                return await baseRepository.GetContentRating(id);
            });
        }

        public async Task<ContentPartyRating?> GetContentRating(Guid roomId, Guid contentId)
        {
            return await baseRepository.GetContentRating(roomId, contentId);
        }

        public async Task<IEnumerable<ContentPartyRating>> GetContentRatingsByRoom(Guid roomId)
        {
            return await baseRepository.GetContentRatingsByRoom(roomId);
        }

        public ContentPartyRating Update(ContentPartyRating contentRating)
        {
            memoryCache.Set(GetKeyById(contentRating.Id), contentRating, cacheEntryOptions);
            return baseRepository.Update(contentRating);
        }

        private static string GetKeyById(Guid id)
        {
            return ContentPartyRatingKey + id.ToString();
        }
    }
}
