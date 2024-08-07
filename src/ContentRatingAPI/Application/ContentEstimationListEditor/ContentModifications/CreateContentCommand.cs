using ContentRating.Domain.Shared.Content;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public record CreateContentCommand(Guid RoomId, Guid EditorId, Guid ContentId, string Name,
        string Url, ContentType ContentType) : IRequest<Result<bool>>;
}
