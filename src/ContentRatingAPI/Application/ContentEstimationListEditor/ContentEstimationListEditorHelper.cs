using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using ContentEstimationListEditorAggregate = ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.ContentEstimationListEditor;

namespace ContentRatingAPI.Application.ContentEstimationListEditor
{
    public static class ContentEstimationListEditorHelper
    {
        public static ContentEditor? TryGetEditorFromRoom(this ContentEstimationListEditorAggregate contentEditorRoom, Guid editorId)
        {
            if (contentEditorRoom.RoomCreator.Id == editorId)
                return contentEditorRoom.RoomCreator;
            return contentEditorRoom.InvitedEditors.FirstOrDefault(c => c.Id == editorId);
        }
    }
}
