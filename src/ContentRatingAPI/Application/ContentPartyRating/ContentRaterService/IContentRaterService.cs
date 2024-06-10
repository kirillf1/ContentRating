using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyRating.ContentRaterService
{
    public interface IContentRaterService
    {
        public Task<ContentRater> GetRaterFromRoom(Guid roomId, Guid raterId);
        public Task<ContentRatersForEstimation> GetContentRatersForEstimation(Guid roomId, Guid initiatorRaterId, Guid targetRaterId);
    }
}
