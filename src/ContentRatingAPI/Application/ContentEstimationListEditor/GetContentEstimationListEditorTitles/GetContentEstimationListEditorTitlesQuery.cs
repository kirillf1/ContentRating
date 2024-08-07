namespace ContentRatingAPI.Application.ContentEstimationListEditor.GetContentEstimationListEditorTitles
{
    public record class GetContentEstimationListEditorTitlesQuery(Guid EditorId) : IRequest<Result<IEnumerable<ContentEstimationListEditorTitle>>>;

}
