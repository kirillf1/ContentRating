using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class ContentUpdatedInRoomDomainEventHandler : INotificationHandler<ContentUpdatedInRoomDomainEvent>
    {
        public Task Handle(ContentUpdatedInRoomDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
