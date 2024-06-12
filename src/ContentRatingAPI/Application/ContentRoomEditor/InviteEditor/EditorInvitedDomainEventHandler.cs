using ContentRating.Domain.AggregatesModel.ContentRoomEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentRoomEditor.InviteEditor
{
    public class EditorInvitedDomainEventHandler : INotificationHandler<EditorInvitedDomainEvent>
    {
        public Task Handle(EditorInvitedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
