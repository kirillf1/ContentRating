namespace ContentRatingAPI.Application.ContentRoomEditor.CompleteContentEstimation
{
    public record CompleteContentEstimationCommand(Guid InitiatorId, Guid RoomId): IRequest;
}
