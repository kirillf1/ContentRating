using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentRemovedFromListDomainEventHandler : INotificationHandler<ContentRemovedFromListDomainEvent>
    {
        private readonly IContentEstimationListEditorNotificationService notificationService;

        public ContentRemovedFromListDomainEventHandler(IContentEstimationListEditorNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        public async Task Handle(ContentRemovedFromListDomainEvent notification, CancellationToken cancellationToken)
        {
            await notificationService.NotifyContentDeleted(notification.ContentListId, notification.Content.Id);
        }
    }
}
