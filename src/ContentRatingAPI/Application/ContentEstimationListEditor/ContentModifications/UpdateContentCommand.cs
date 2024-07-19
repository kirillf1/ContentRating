using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public record UpdateContentCommand(Guid Id, Guid RoomId, Guid EditorId, string Name, string Url, ContentType ContentType) : IRequest<Result<bool>>;
}
