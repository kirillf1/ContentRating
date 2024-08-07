namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public record RemoveContentCommand(Guid ContentId, Guid RoomId, Guid EditorId) : IRequest<Result>;
}
