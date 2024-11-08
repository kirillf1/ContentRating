// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRating.Domain.Shared;
using ContentRatingAPI.Infrastructure.Data.Caching;

namespace ContentRatingAPI.Infrastructure.Data.Repositories
{
    public class CachingContentPartyRatingRepository : IContentPartyRatingRepository
    {
        private readonly IContentPartyRatingRepository baseRepository;
        private readonly GenericCacheBase<ContentPartyRating> cache;

        public CachingContentPartyRatingRepository(IContentPartyRatingRepository baseRepository, GenericCacheBase<ContentPartyRating> genericCache)
        {
            this.baseRepository = baseRepository;
            cache = genericCache;
        }

        public IUnitOfWork UnitOfWork => baseRepository.UnitOfWork;

        public ContentPartyRating Add(ContentPartyRating contentRating)
        {
            cache.Set(contentRating.Id, contentRating);
            return baseRepository.Add(contentRating);
        }

        public void Delete(ContentPartyRating contentRating)
        {
            cache.Remove(contentRating.Id);
            baseRepository.Delete(contentRating);
        }

        public async Task<ContentPartyRating?> GetContentRating(Guid id)
        {
            if (cache.TryGetValue(id, out var rating) && rating is not null)
            {
                return rating;
            }

            rating = await baseRepository.GetContentRating(id);

            if (rating is not null)
            {
                cache.Set(id, rating);
            }

            return rating;
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
            cache.Set(contentRating.Id, contentRating);
            return baseRepository.Update(contentRating);
        }
    }
}
