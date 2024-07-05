namespace ContentRatingAPI.Application.ContentRoomEditor.KickEditor
{
    public record KickEditorCommand(Guid RoomId, Guid InitiatorId, Guid TargetEditorId) : IRequest<Result>;
}
