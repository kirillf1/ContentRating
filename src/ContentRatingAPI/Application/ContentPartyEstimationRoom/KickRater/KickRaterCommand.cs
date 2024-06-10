namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.KickRater
{
    public record KickRaterCommand(Guid RoomId, Guid KickInitiatorId, Guid RaterForKickId) : IRequest;

}
