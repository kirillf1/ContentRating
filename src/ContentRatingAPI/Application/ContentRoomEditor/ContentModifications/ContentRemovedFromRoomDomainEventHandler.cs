using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class ContentRemovedFromRoomDomainEventHandler : INotificationHandler<ContentRemovedFromRoomDomainEvent>
    {
        public Task Handle(ContentRemovedFromRoomDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
