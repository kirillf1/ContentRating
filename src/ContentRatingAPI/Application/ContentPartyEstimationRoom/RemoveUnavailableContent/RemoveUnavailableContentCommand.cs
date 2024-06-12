namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent
{
    public record RemoveUnavailableContentCommand(Guid RoomId, Guid ContentId, Guid RemoveContentInitiatorId) : IRequest;
}
