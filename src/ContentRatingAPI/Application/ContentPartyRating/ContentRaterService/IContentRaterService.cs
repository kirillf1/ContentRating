// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyRating.ContentRaterService
{
    public interface IContentRaterService
    {
        public Task<ContentRater> GetRaterFromRoom(Guid roomId, Guid raterId);
        public Task<ContentRatersForEstimation> GetContentRatersForEstimation(Guid roomId, Guid initiatorRaterId, Guid targetRaterId);
    }
}
