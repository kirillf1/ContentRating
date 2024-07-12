namespace ContentRatingAPI.Application.ContentEstimationListEditor.KickEditor
{
    public record KickEditorCommand(Guid RoomId, Guid InitiatorId, Guid TargetEditorId) : IRequest<Result>;
}
