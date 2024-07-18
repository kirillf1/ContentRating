using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.ChangeRatingRange
{
    public class RatingRangeChangedDomainEventHandler : INotificationHandler<RatingRangeChangedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;
        private readonly IContentPartyEstimationNotificationService notificationService;

        public RatingRangeChangedDomainEventHandler(ContentPartyRatingService contentPartyRatingService, IContentPartyEstimationNotificationService notificationService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
            this.notificationService = notificationService;
        }
        public async Task Handle(RatingRangeChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            var minScore = new Score(notification.NewMinRating.Value);
            var maxScore = new Score(notification.NewMaxRating.Value);

            await contentPartyRatingService.ChangeRatingSpecification(notification.RoomId, minScore, maxScore);

            await notificationService.NotifyRatingRangeChanged(notification.RoomId, minScore.Value, maxScore.Value);
        }
    }
}
