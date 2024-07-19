namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentAddedToListDomainEvent(Content NewContent, Guid ContentListId) : INotification;

}
