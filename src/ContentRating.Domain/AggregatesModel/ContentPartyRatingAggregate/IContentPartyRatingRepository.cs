﻿namespace ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate
{
    public interface IContentPartyRatingRepository : IRepository<ContentPartyRating>
    {
        ContentPartyRating Add(ContentPartyRating contentRating);
        ContentPartyRating Update(ContentPartyRating contentRating);
        void Delete(ContentPartyRating contentRating);
        Task<ContentPartyRating> GetContentRating(Guid id);
        Task<IEnumerable<ContentPartyRating>> GetContentRatingsByRoom(Guid roomId);

    }
}
