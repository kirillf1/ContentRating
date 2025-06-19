using Ardalis.Result;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.DeleteContentEstimationListEditor
{
    public record DeleteContentEstimationListEditorCommand(Guid RoomId, Guid UserId) 
        : IRequest<Result<bool>>;
} 