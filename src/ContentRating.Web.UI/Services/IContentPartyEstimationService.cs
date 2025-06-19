using ContentRating.Web.Contracts.ContentPartyEstimationRoom;

namespace ContentRating.Web.UI.Services
{
    public interface IContentPartyEstimationService
    {
        Task<IEnumerable<PartyEstimationTitle>?> GetRoomsAsync(bool includeEstimated = true, bool includeNotEstimated = true);
        Task<bool> CreateRoomAsync(CreatePartyEstimationRoomRequest request);
        Task<PartyEstimationRoomResponse?> GetRoomAsync(Guid roomId);
        Task<bool> InviteRaterAsync(Guid roomId, InviteRaterRequest request);
        Task<bool> KickRaterAsync(Guid roomId, Guid raterId);
        Task<bool> CompleteEstimationAsync(Guid roomId);
        Task<bool> RemoveContentAsync(Guid roomId, Guid contentId);
        Task<bool> ChangeRatingRangeAsync(Guid roomId, ChangeRatingRangeRequest request);
        Task<bool> DeleteRoomAsync(Guid roomId);
    }
} 