namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record EditorInvitedDomainEvent(Guid RoomId, ContentEditor Inviter, ContentEditor NewEditor) : INotification;
}
