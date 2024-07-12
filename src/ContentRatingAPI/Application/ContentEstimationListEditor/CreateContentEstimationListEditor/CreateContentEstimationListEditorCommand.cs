namespace ContentRatingAPI.Application.ContentEstimationListEditor.CreateContentEstimationListEditor
{
    public record CreateContentEstimationListEditorCommand(Guid Id, Guid CreatorId, string CreatorName, string RoomName) : IRequest<Result>;
}
