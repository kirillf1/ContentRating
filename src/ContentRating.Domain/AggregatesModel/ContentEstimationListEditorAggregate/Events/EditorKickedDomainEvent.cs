namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record EditorKickedDomainEvent(Guid ContentListId, ContentEditor Initiator, ContentEditor KickedEditor) : INotification;
}
