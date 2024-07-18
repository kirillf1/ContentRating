using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.ContentModifications
{
    public class ContentAddedToListDomainEventHandler : INotificationHandler<ContentAddedToListDomainEvent>
    {
        public Task Handle(ContentAddedToListDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
