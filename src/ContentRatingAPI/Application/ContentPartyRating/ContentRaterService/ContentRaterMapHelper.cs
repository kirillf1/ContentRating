using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using MediatR;

namespace ContentRatingAPI.Application.ContentPartyRating.ContentRaterService
{
    public static class ContentRaterMapHelper 
    {
        public static ContentRater MapToContentRater(this Rater rater)
        {
            var raterType = rater.Role switch
            {
                RoleType.Admin => RaterType.Admin,
                RoleType.Default => RaterType.Default,
                RoleType.Mock => RaterType.Mock,
                _ => throw new NotImplementedException(),
            };
            return new ContentRater(rater.Id, raterType);
        }
    }
}
