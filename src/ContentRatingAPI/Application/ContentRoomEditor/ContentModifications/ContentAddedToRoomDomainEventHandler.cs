using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentRoomEditor.ContentModifications
{
    public class ContentAddedToRoomDomainEventHandler : INotificationHandler<ContentAddedToRoomDomainEvent>
    {
        public Task Handle(ContentAddedToRoomDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
