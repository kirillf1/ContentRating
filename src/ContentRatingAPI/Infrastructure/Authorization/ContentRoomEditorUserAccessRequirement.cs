using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentRoomEditorUserAccessRequirement : IAuthorizationRequirement
    {
        public async Task<bool> Pass(IContentEditorRoomRepository contentEditorRoomRepository, Guid editorId, Guid roomId)
        {
            return await contentEditorRoomRepository.HasEditorInRoom(roomId, editorId);
        }
    }
}
