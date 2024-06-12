using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using ContentEditorRoomAggregate = ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.ContentRoomEditor;

namespace ContentRatingAPI.Application.ContentRoomEditor
{
    public static class ContentRoomEditorHelper
    {
        public static Editor? TryGetEditorFromRoom(this ContentEditorRoomAggregate contentEditorRoom, Guid editorId)
        {
            if (contentEditorRoom.RoomCreator.Id == editorId)
                return contentEditorRoom.RoomCreator;
            return contentEditorRoom.InvitedEditors.FirstOrDefault(c=> c.Id == editorId);
        }
    }
}
