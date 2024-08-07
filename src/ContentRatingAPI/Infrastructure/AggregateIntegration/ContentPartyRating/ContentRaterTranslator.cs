using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;

namespace ContentRatingAPI.Infrastructure.AggregateIntegration.ContentPartyRating
{
    public class ContentRaterTranslator
    {
        public ContentRater Translate(Rater rater)
        {
            var role = rater.Role;
            var raterType = role switch
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
