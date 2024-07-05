using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.InviteRater
{
    public record InviteRaterCommand(Guid RoomId, Guid InviteInitiatorId, Guid RaterForInviteId, 
        RoleType RoleType, string RaterName) : IRequest<Result>;
}
