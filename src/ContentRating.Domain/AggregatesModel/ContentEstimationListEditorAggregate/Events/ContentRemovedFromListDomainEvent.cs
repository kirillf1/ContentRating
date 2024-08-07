namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentRemovedFromListDomainEvent(Content Content, Guid ContentListId) : INotification;
}
