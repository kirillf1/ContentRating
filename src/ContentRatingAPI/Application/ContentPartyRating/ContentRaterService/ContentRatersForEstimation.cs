using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Application.ContentPartyRating.ContentRaterService
{
    public record ContentRatersForEstimation(ContentRater Initiator, ContentRater TargetRater);
}
