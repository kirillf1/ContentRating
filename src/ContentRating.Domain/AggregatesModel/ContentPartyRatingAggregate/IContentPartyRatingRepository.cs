// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public interface IContentPartyRatingRepository : IRepository<ContentPartyRating>
    {
        ContentPartyRating Add(ContentPartyRating contentRating);
        ContentPartyRating Update(ContentPartyRating contentRating);
        void Delete(ContentPartyRating contentRating);
        Task<ContentPartyRating?> GetContentRating(Guid id);
        Task<ContentPartyRating?> GetContentRating(Guid roomId, Guid contentId);
        Task<IEnumerable<ContentPartyRating>> GetContentRatingsByRoom(Guid roomId);
    }
}
