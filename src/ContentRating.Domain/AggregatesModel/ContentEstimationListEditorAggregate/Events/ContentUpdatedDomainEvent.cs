using ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate;

namespace ContentRating.Domain.AggregatesModel.ContentEstimationListEditorAggregate.Events
{
    public record ContentUpdatedDomainEvent(Content UpdatedContent, Guid ContentListId) : INotification;
}
