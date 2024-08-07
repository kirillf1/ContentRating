using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentEstimationListEditorUserAccessRequirement : IAuthorizationRequirement
    {
        public async Task<bool> Pass(IContentEstimationListEditorRepository contentEditorRoomRepository, Guid editorId, Guid roomId)
        {
            return await contentEditorRoomRepository.HasEditorInContentEstimationList(roomId, editorId);
        }
    }
}
