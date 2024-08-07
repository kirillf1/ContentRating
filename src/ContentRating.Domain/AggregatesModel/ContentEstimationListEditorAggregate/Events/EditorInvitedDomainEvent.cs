namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record EditorInvitedDomainEvent(Guid ContentListId, ContentEditor Inviter, ContentEditor NewEditor) : INotification;
}
