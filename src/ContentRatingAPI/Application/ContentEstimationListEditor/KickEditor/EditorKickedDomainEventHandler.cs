using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events;

namespace ContentRatingAPI.Application.ContentEstimationListEditor.KickEditor
{
    public class EditorKickedDomainEventHandler : INotificationHandler<EditorKickedDomainEvent>
    {
        public Task Handle(EditorKickedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
