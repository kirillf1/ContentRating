using ContentRating.Domain.AggregatesModel.ContentPartyEstimationRoomAggregate.Events;
using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate;
using ContentRatingAPI.Application.Notifications.IContentPartyEstimationNotifications;

namespace ContentRatingAPI.Application.ContentPartyEstimationRoom.RemoveUnavailableContent
{
    public class UnavailableContentRemovedDomainEventHandler : INotificationHandler<UnavailableContentRemovedDomainEvent>
    {
        private readonly IContentPartyRatingRepository contentPartyRatingRepository;
        private readonly IContentPartyEstimationNotificationService notificationService;

        public UnavailableContentRemovedDomainEventHandler(IContentPartyRatingRepository contentPartyRatingRepository, IContentPartyEstimationNotificationService notificationService)
        {
            this.contentPartyRatingRepository = contentPartyRatingRepository;
            this.notificationService = notificationService;
        }
        public async Task Handle(UnavailableContentRemovedDomainEvent notification, CancellationToken cancellationToken)
        {
            var contentRating = await contentPartyRatingRepository.GetContentRating(notification.RoomId, notification.RemovedContent.Id);
            if (contentRating is not null)
            {
                contentPartyRatingRepository.Delete(contentRating);
                await contentPartyRatingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }

            await notificationService.NotifyContentDeleted(notification.RoomId, notification.RemovedContent.Id);
        }
    }
}
