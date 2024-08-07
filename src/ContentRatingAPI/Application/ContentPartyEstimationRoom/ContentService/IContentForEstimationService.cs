using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.ContentService
{
    public interface IContentForEstimationService
    {
        public Task<IEnumerable<ContentForEstimation>> RequestContentForEstimationFromEditor(Guid contentListId, Guid raterId);
    }
}
