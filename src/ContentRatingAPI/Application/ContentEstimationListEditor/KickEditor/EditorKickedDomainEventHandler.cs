using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;
using ContentRatingAPI.Application.Notifications.IContentEstimationListEditorNotifications;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.KickEditor
{
    public class EditorKickedDomainEventHandler : INotificationHandler<EditorKickedDomainEvent>
    {
        private readonly IContentEstimationListEditorNotificationService notificationService;

        public EditorKickedDomainEventHandler(IContentEstimationListEditorNotificationService notificationService)
        {
            this.notificationService = notificationService;
        }
        public async Task Handle(EditorKickedDomainEvent notification, CancellationToken cancellationToken)
        {
            await notificationService.NotifyEditorKicked(notification.ContentListId, notification.KickedEditor.Id, notification.Initiator.Id);
        }
    }
}
