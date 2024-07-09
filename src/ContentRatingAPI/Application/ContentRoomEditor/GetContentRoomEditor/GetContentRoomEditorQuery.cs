namespace ContentRatingAPI.Application.ContentRoomEditor.GetContentRoomEditor
{
    public record class GetContentRoomEditorQuery(Guid RoomId) : IRequest<Result<ContentRoomEditorResponse>>;
}
