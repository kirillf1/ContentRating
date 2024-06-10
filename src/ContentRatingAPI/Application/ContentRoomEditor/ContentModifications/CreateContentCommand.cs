using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public record CreateContentCommand(Guid Id, Guid RoomId, Guid EditorId, string Name, string Path, ContentType ContentType);
}
