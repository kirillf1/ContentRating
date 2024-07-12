using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentUpdatedInRoomDomainEventHandler : INotificationHandler<ContentUpdatedInRoomDomainEvent>
    {
        public Task Handle(ContentUpdatedInRoomDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
