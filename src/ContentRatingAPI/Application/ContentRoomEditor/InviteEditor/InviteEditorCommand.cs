namespace ContentRatingAPI.Application.ContentRoomEditor.InviteEditor
{
    public record InviteEditorCommand(Guid RoomId, Guid InitiatorId, Guid NewEditorId, string NewEditorName) : IRequest<Result>;
}
