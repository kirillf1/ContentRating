using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate;
using Microsoft.AspNetCore.Authorization;

namespace ContentRatingAPI.Infrastructure.Authorization
{
    public class ContentPartyEstimationRoomRaterAccessRequirement : IAuthorizationRequirement
    {
        public async Task<bool> Pass(IContentPartyEstimationRoomRepository contentPartyEstimationRoomRepository, Guid raterId, Guid roomId)
        {
            return await contentPartyEstimationRoomRepository.HasRaterInRoom(roomId, raterId);
        }
    }
}
