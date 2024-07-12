namespace ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor
{
    public record InviteEditorCommand(Guid RoomId, Guid InitiatorId, Guid NewEditorId, string NewEditorName) : IRequest<Result>;
}
