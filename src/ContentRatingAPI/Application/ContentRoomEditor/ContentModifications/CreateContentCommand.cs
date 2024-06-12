using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public record CreateContentCommand(Guid RoomId, Guid EditorId, Guid ContentId, string Name, string Path, ContentType ContentType) : IRequest;
}
