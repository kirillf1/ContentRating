namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditor
{
    public record class GetContentEstimationListEditorQuery(Guid RoomId) : IRequest<Result<ContentEstimationListEditorResponse>>;
}
