using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;
using Microsoft.AspNetCore.SignalR;

namespace ContentRatingAPI.Hubs.NotificationServices
{
    public class ContentPartyEstimationNotificationHubService : IContentPartyEstimationNotificationService
    {
        private readonly IHubContext<ContentPartyEstimationHub> hubContext;

        public ContentPartyEstimationNotificationHubService(IHubContext<ContentPartyEstimationHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public async Task NotifyContentDeleted(Guid roomId, Guid contentId)
        {
            await hubContext.Clients.Group(roomId.ToString()).SendAsync("ContentDeleted", contentId);
        }

        public async Task NotifyEstimationCompleted(Guid roomId)
        {
            await hubContext.Clients.Group(roomId.ToString()).SendAsync("EstimationCompleted");
        }

        public async Task NotifyRaterInvited(Guid roomId, Guid newRaterId, string raterName, double baseScore)
        {
            await hubContext.Clients.Group(roomId.ToString()).SendAsync("RaterInvited", newRaterId, raterName, baseScore);
        }

        public async Task NotifyRaterKicked(Guid roomId, Guid kickedRaterId)
        {
            await hubContext.Clients.Group(roomId.ToString()).SendAsync("RaterKicked", kickedRaterId);
        }

        public async Task NotifyRatingChanged(Guid roomId, Guid raterId, Guid ratingId, double score)
        {
            await hubContext.Clients.Group(roomId.ToString()).SendAsync("RatingChanged", raterId, ratingId, score);
        }

        public async Task NotifyRatingRangeChanged(Guid roomId, double minRating, double maxRating)
        {
            await hubContext.Clients.Group(roomId.ToString()).SendAsync("RatingRangeChanged", minRating, maxRating);
        }
    }
}
