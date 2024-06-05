using ContentRating.Domain.AggregatesModel.ContentRatingAggregate.Events;
using ContentRatingAPI.Notifications;

namespace ContentRatingAPI.Application.ContentRating.EstimateContent
{
    public class ContentEstimatedDomainEventHandler : INotificationHandler<ContentRatingChangedDomainEvent>
    {
        private readonly IContentRatingNotificationService contentRatingNotificationService;

        public ContentEstimatedDomainEventHandler(IContentRatingNotificationService contentRatingNotificationService)
        {
            this.contentRatingNotificationService = contentRatingNotificationService;
        }
        public Task Handle(ContentRatingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            // Логика связанная с message bus и интеграционными событиями
            // Обработка других контекстов
            contentRatingNotificationService.NotififyRatingChanged(notification.RoomId, notification.Rater.Id, notification.Rater.CurrentScore.Value);
        }
    }
}
