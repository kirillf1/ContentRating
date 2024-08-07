using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.CompleteContentEstimation
{
    public class AllContentEstimatedDomainEventHandler : INotificationHandler<AllContentEstimatedDomainEvent>
    {
        private readonly ContentPartyRatingService contentPartyRatingService;
        private readonly IContentPartyEstimationNotificationService notificationService;

        public AllContentEstimatedDomainEventHandler(ContentPartyRatingService contentPartyRatingService, IContentPartyEstimationNotificationService notificationService)
        {
            this.contentPartyRatingService = contentPartyRatingService;
            this.notificationService = notificationService;
        }
        public async Task Handle(AllContentEstimatedDomainEvent notification, CancellationToken cancellationToken)
        {
            await contentPartyRatingService.CompleteContentEstimation(notification.RoomId);
            
            await notificationService.NotifyEstimationCompleted(notification.RoomId);
        }
    }
}
