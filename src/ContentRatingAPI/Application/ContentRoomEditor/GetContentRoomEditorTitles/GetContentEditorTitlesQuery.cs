namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditorTitles
{
    public record class GetContentEditorTitlesQuery(Guid EditorId): IRequest<Result<IEnumerable<ContentRoomEditorTitle>>>; 
   
}
