// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.ContentPartyRating.ContentRaterService;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating
{
    public class TranslatingContentRaterService : IContentRaterService
    {
        private readonly ContentRaterAdapter contentRaterAdapter;

        public TranslatingContentRaterService(ContentRaterAdapter contentRaterAdapter)
        {
            this.contentRaterAdapter = contentRaterAdapter;
        }

        public async Task<ContentRatersForEstimation> GetContentRatersForEstimation(Guid roomId, Guid initiatorRaterId, Guid targetRaterId)
        {
            var raters = await contentRaterAdapter.GetContentRates(roomId, initiatorRaterId, targetRaterId);
            if (raters.Count == 1)
            {
                return new ContentRatersForEstimation(raters[0], raters[0]);
            }

            return new ContentRatersForEstimation(raters[0], raters[1]);
        }

        public async Task<ContentRater> GetRaterFromRoom(Guid roomId, Guid RaterId)
        {
            var rater = await contentRaterAdapter.GetContentRater(roomId, RaterId);
            return rater;
        }
    }
}
