using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentRoomEditor.KickEditor
{
    public class EditorKickedDomainEventHandler : INotificationHandler<EditorKickedDomainEvent>
    {
        public Task Handle(EditorKickedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
