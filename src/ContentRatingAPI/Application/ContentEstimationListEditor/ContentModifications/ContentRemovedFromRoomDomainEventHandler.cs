using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentRemovedFromRoomDomainEventHandler : INotificationHandler<ContentRemovedFromRoomDomainEvent>
    {
        public Task Handle(ContentRemovedFromRoomDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
