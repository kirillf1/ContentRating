using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.InviteEditor
{
    public class EditorInvitedDomainEventHandler : INotificationHandler<EditorInvitedDomainEvent>
    {
        public Task Handle(EditorInvitedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
