using ContentRating.Domain.AggregatesModel.ContentPartyRatingAggregate.Events;

namespace ContentRatingAPI.Application.ContentPartyRating.EstimateContent
{
    public class ContentRatingChangedDomainEventHandler : INotificationHandler<ContentRatingChangedDomainEvent>
    {
        public Task Handle(ContentRatingChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;   
        }
    }
}
