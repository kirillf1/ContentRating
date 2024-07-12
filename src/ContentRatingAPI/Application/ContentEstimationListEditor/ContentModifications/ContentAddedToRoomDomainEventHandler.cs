using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentAddedToRoomDomainEventHandler : INotificationHandler<ContentAddedToRoomDomainEvent>
    {
        public Task Handle(ContentAddedToRoomDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
