using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class ContentRatingChangedDomainEventHandler : INotificationHandler<ContentRatingChangedDomainEvent>
    {
        private readonly IContentPartyEstimationNotificationService notificationService;

        public ContentRatingChangedDomainEventHandler(IContentPartyEstimationNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        public async Task Handle(ContentRatingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var roomId = notification.RoomId;
            var raterId = notification.Rater.RaterId;
            var ratingId = notification.ContentRatingId;
            var newScore = notification.NewRaterScore.Value;

            await notificationService.NotifyRatingChanged(roomId, raterId, ratingId, newScore);   
        }
    }
}
