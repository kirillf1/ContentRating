namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.CompleteContentEstimation
{
    public record CompleteContentEstimationCommand(Guid InitiatorId, Guid RoomId) : IRequest;
}
