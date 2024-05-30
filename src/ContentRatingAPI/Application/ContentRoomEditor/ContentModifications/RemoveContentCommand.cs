namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public record RemoveContentCommand(Guid ContentId, Guid RoomId, Guid EditorId) : IRequest;
}
