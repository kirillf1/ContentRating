using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public class InviteRaterRequest
    {
        public required RoleType RoleType { get; set; }
        public required string RaterName { get; set; }
    }
}
