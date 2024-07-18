using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentRemovedFromListDomainEventHandler : INotificationHandler<ContentRemovedFromListDomainEvent>
    {
        public Task Handle(ContentRemovedFromListDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
